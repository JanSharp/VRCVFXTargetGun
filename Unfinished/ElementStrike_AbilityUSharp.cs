using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    public class ElementStrike_AbilityUSharp : UdonSharpBehaviour
    {
        public Transform destination;
        public float maxdistance = 10f;
        public Transform aimPoint;
        public VRC_Pickup pickup;
        public int updaterate = 2;
        public LayerMask layermask;
        public Vector3 updirection;
        public float offset;
        public Renderer[] visuals;
        public ParticleSystem Spell_FX;
        public Transform Spell_FX_Transform;
        public float Spell_CD;
        private bool isCooldDown = false;

        public override void OnPickupUseDown()
        {
            RaycastHit hitinfo;
            if (!isCooldDown && Physics.Raycast(aimPoint.position, aimPoint.rotation * Vector3.forward, out hitinfo, maxdistance, layermask.value))
            {
                isCooldDown = true;
                if (Spell_FX != null)
                {
                    Spell_FX_Transform.SetPositionAndRotation(hitinfo.point, Quaternion.LookRotation(hitinfo.normal));
                    destination.gameObject.SetActive(false);
                    Spell_FX.Play();
                    SendCustomEventDelayedSeconds(nameof(SpellCD), Spell_CD);
                }
            }
        }

        public void SpellCD()
        {
            destination.gameObject.SetActive(true);
            isCooldDown = false;
        }

        private void Update()
        {
            if ((Time.frameCount % updaterate) == 0)
            {
                RaycastHit hitinfo;
                if (Physics.Raycast(aimPoint.position, aimPoint.rotation * Vector3.forward, out hitinfo, maxdistance, layermask.value))
                {
                    for (int i = 0; i < visuals.Length; i++)
                    {
                        if (!visuals[i].enabled)
                        {
                            visuals[i].enabled = true;
                        }
                    }
                    Quaternion destinationRotation = Quaternion.FromToRotation(updirection, hitinfo.normal);
                    destination.rotation = destinationRotation;
                    destination.position = hitinfo.point + ((destinationRotation * updirection) * offset);
                }
                else
                {
                    visdissable();
                }
            }
        }

        private void visdissable()
        {
            for (int i = 0; i < visuals.Length; i++)
            {
                if (visuals[i].enabled)
                {
                    visuals[i].enabled = false;
                }
            }
        }
    }
}
