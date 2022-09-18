using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

///cSpell:ignore chipz

namespace JanSharp
{
    public class Controller_2d : UdonSharpBehaviour
    {
        public Rigidbody2D characterRB;
        public Animator characterAnim;
        public UdonBehaviour chipz2DHP;
        public float speed;
        public float jumpImpulse;
        private bool isScarChan;
        [FieldChangeCallback(nameof(onHorizontalValueChange))]
        private float horizontalValue;

        private float onHorizontalValueChange
        {
            set
            {
                characterAnim.SetFloat("Speed", Mathf.Abs(value));
                horizontalValue = value;
            }
        }

        // public float maxVelocity;
        // public float secondsUntilFullSpeed;
        // public void FixedUpdate()
        // {
        //     // step 1
        //     // slowly start moving
        //     Vector2 currentVelocity = characterRB.velocity;
        //     // currentVelocity.x = currentVelocity.x + 1; // something
        //     // how much do we actually need to change?
        //     float velocityDeltaPerSecond = maxVelocity / secondsUntilFullSpeed;
        //     float velocityDelta = velocityDeltaPerSecond * Time.fixedDeltaTime;
        //     currentVelocity.x = currentVelocity.x + velocityDelta;
        //     // characterRB.velocity = currentVelocity;
        //     // but now we need to handle maxVelocity
        //     currentVelocity.x = Mathf.Min(currentVelocity.x, maxVelocity);
        //     characterRB.velocity = currentVelocity;

        //     // this is linearly changing velocity which will have non linear effect on speed

        //     // ok so first keep it simple and stupid, then
        //     // figure out the change based on fields and deltatime
        //     // limit resulting velocity to max velocity
        //     // order of the last 2 doesn't matter

        //     // now do this for both going left and right
        //     // going left is slightly different because we have to do everything in the negative value range

        //     // alright, whatever. I mean this isn't realistic anyway
        //     // besides using AddForce to achieve the same thing would just make it more complex...
        //     // and like I said, it would be achieveing the same thing

        //     // alright, with left and right done we need to return to zero velocity
        //     // but not on key up, only when both keys are held or both keys are released
        //     // this is the part that was for some reason so hard to explain, but idk how else to explain it...
        //     // there must be a way

        //     // well, let's keep it simple.
        //     // **under which exact circumstances does it need to... **
        //     // SIMPLE I said...
        //     // but _HOW_

        //     // Idk, I think I just have to wing it. There is no simple straight forward way to explain it
        //     // it just makes sense in my brain and I don't know how to put that kind of logic into words
        //     // actually, what about looking at a graph!
        // }
    }
}
