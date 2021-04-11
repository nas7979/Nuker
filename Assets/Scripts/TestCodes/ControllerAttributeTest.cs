using UnityEngine;

namespace Game.TEST
{
    // Used Cases

    // Operate by '[Controller]' annotation release

    //[Controller]
    public class TEST : IController
    {
        public void Initialize()
        {
            Debug.Log("Initalization TEST");
        }

        public void FrameMove()
        {
            Debug.Log("FrameMove TEST");
        }
    }

    //[Controller]
    public class TEST01 : IController
    {
        public void Initialize()
        {
            Debug.Log("Initalization TEST01");
        }

        public void FrameMove()
        {
            Debug.Log("FrameMove TEST01");
        }
    }

    //[Controller]
    public class TEST02 : IController
    {
        public void Initialize()
        {
            Debug.Log("Initalization TEST02");
        }

        public void FrameMove()
        {
            Debug.Log("FrameMove TEST02");
        }
    }

    // =========== Caution =========== 

    // Error, Please Put IController Interface

    //[Controller]
    //public class TEST03
    //{
    //    public void Initialize()
    //    {
    //        Debug.Log("Initalization TEST03");
    //    }
    //
    //    public void FrameMove()
    //    {
    //        Debug.Log("FrameMove TEST03");
    //    }
    //}


    // Error, Please don't used Unity's Component in labeled Controller
    // Only usable the class that be allocated it as 'new' Keyword

    //[Controller]
    //public class TEST04 : MonoBehaviour, IController
    //{
    //    private Rigidbody2D rb;
    //    public void Initialize()
    //    {
    //        rb.GetComponent<Rigidbody2D>();     // Error
    //        Debug.Log("Initalization TEST04");
    //    }

    //    public void FrameMove()
    //    {
    //        Debug.Log("FrameMove TEST04");
    //    }
    //}
}