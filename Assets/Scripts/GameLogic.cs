using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Game.Controller;
using UnityEngine;

/*
 *  Usage
 *  
 *  # 원하는 클래스에 FrameMove나 Initalize 함수를 콜백받고 싶을 때
 *  
 *  ## Case 01 : Subscribe 매소드를 이용한 이벤트 등록 (권장)
 *  
 *  - 장점
 *      - Initalize 따로 FrameMove 따로 등록이 가능하다. (그냥 원하는 함수있으면 등록하면 됨)
 *  - 단점
 *      - Subscribe 등록 시 반드시 UnSubscribe를 해줘야 한다. (혹시나 모를 경우를 대비해서..)
 *  
 *  ```
 *  public class SomethingClass : IController <- 필수는 아니지만 가독성을 위해서 써주면 더 좋고.. 쓰지 않아도 상관없음
 *  {
 *      private void OnEnable()
 *      {
 *          // 콜백을 받기 위해서 필수로 사용해야 됨 (둘중에 둘중에 하나만 등록해도 된다)
 *          GameSystem.Instance.SubscribeInitalizeListener(Initialize);     // 등록
 *          GameSystem.Instance.SubscribeUpdateListener(FrameMove);
 *      }
 *      
 *      private void OnDisable()
 *      {
 *          // 오브젝트가 비활성 상태일 때 콜백 등록 해제를 통해서 씬을 다시 로드했을 때의 중복 등록을 방지함
 *          GameSystem.Instance.UnSubscribeInitalizeListener(Initialize);   // 해제
 *          GameSystem.Instance.UnSubscribeUpdateListener(FrameMove);
 *      }
 *      
 *      public void Initialize()
 *      {
 *          something..
 *      }
 *  
 *      public void FrameMove()
 *      {
 *          something..
 *      }
 *  }
 * 
 * ```
 * 
 * ### 알아두면 좋은 것
 * - Main Loop 실행 순서
 *  - Awake -> OnEnable -> Start -> Update -> LateUpdate 이후에 실행됨
 *  - 참고 : https://docs.unity3d.com/Manual/ExecutionOrder.html
 * 
 * - Subscribe, UnSubscribe Method
 *  - void 형태의 함수만 등록 가능
 *  - 람다를 통한 등록, 해제도 가능 (ex: SubscribeInitalizeListener(() => { Debug.Log("HelloWorld"); });
 * 
 * 
 * 
 * ## Case 02 : 속성을 이용한 등록
 * 
 * - 장점
 *  - 사용이 간단하다 (그냥 IController 받고 클래스위에 [Controller] 만 붙이면 됨
 * - 단점
 *  - 안정성 보장 못함 (예상치 못한 버그가 생길 수 있음)
 *  - 퍼포먼스 관련해서 문제가 생길수도 있음
 * 
 * ### 사용법
 * - 등록하고자 하는 클래스에 IController 인터페이스를 받고 오버라이드를 모두 해주고 해당 클래스 위에 [Controller]를 붙이면 끝
 * - 예시자료는 Assets/Scripts/TestCodes/ControllerAttributeTest.cs 에서 확인할 수 있음
 * 
 * 
 */

public interface IController
{
    void Initialize();
    void FrameMove();
}

[Serializable]
[DisallowMultipleComponent]
public class GameLogic : MonoBehaviour
{
    GameSystem m_gameSystem = null;
    private void Awake()
    {
        m_gameSystem = GameSystem.Instance;


        m_gameSystem.CreateController();

        m_gameSystem.Setup();
    }
    
    private void OnEnable()
    {
        m_gameSystem.Initalize();
        StartCoroutine(m_gameSystem.MainLoop());
    }

    private void OnDisable()
    {
        m_gameSystem.Release();
        StopCoroutine(m_gameSystem.MainLoop());
    }
}

[Serializable]
public class GameSystem : Singleton<GameSystem>
{
    private Queue<IController> _controllers;

    public Queue<IController> Controllers
    {
        get
        {
            return _controllers;
        }
        set
        {
            _controllers = value;
        }
    }

    public event Action InitalizeListener;
    public event Action UpdateListener;

    public void SubscribeInitalizeListener(Action action) => InitalizeListener += action;
    public void SubscribeUpdateListener(Action action)  => UpdateListener += action;

    public void UnSubscribeInitalizeListener(Action action) => InitalizeListener -= action;
    public void UnSubscribeUpdateListener(Action action) => UpdateListener -= action;

    public void CreateController() => _controllers = new Queue<IController>();
    
    public void Setup()
    {
        //this.ClearEvent(out InitalizeListener);
        //this.ClearEvent(out UpdateListener);

        this.ClearQueue(Controllers);

        // Find Controller attributes & Enqueue these controller
        this.FindAttributeClass<ControllerAttribute>().ToList().ForEach(e =>
        {
            object activator = Activator.CreateInstance(e);
            var controller = activator as IController;
            this.push((controller != null) ? controller
                : throw new NullReferenceException($"Type {e.Name} doesn't have any IController"));
        });


        
    }

    public void Initalize()
    {
        InitalizeListener += OnInitListener;
        UpdateListener += OnUpdateListener;
    }
    public void Release()
    {
        InitalizeListener -= OnInitListener;
        UpdateListener -= OnUpdateListener;
    }

    private void OnInitListener()
    {
        Controllers.ToList().ForEach(e =>
        {
            e.Initialize();
        });
    }
    private void OnUpdateListener()
    {
        Controllers.ToList().ForEach(e =>
        {
            e.FrameMove();
        });
    }

    public IEnumerator MainLoop()
    {
        yield return new WaitForEndOfFrame();
        this.Raise(InitalizeListener);

        while (true)
        {
            this.Raise(UpdateListener);
            yield return null;
        }
    }
}