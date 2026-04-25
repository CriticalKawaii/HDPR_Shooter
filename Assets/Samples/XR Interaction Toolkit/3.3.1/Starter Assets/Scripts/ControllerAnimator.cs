using Codice.Client.BaseCommands.BranchExplorer;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Component which reads input values and drives the thumbstick, trigger, and grip transforms
    /// to animate a controller model.
    /// </summary>
    public class ControllerAnimator : MonoBehaviour
    {

        [Header("Trigger")]
        [SerializeField] private Transform thumb1;
        [SerializeField] private Transform thumb2;

        [SerializeField] private Transform pointer1;
        [SerializeField] private Transform pointer2;
        [SerializeField] private Transform pointer3;

        [SerializeField] private Transform middle1;
        [SerializeField] private Transform middle2;
        [SerializeField] private Transform middle3;

        [SerializeField] private Transform ring1, ring2, ring3;
        [SerializeField] private Transform pinky1, pinky2, pinky3;


        // --- Thumb ---
        private Quaternion thumb1Start, thumb1End;
        private Quaternion thumb2Start, thumb2End;

        // --- Pointer ---
        private Quaternion pointer1Start, pointer1End;
        private Quaternion pointer2Start, pointer2End;
        private Quaternion pointer3Start, pointer3End;

        // --- Middle ---
        private Quaternion middle1Start, middle1End;
        private Quaternion middle2Start, middle2End;
        private Quaternion middle3Start, middle3End;

        private Quaternion ring1Start, ring2Start, ring3Start;
        private Quaternion ring1End, ring2End, ring3End;

        private Quaternion pinky1Start, pinky2Start, pinky3Start;
        private Quaternion pinky1End, pinky2End, pinky3End;






        [SerializeField]
        XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");


        [SerializeField]
        XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");

        void OnEnable()
        {
            m_TriggerInput?.EnableDirectActionIfModeUsed();
            m_GripInput?.EnableDirectActionIfModeUsed();
        }

        void OnDisable()
        {
            m_TriggerInput?.DisableDirectActionIfModeUsed();
            m_GripInput?.DisableDirectActionIfModeUsed();
        }
        void Start()
        {
            // ===== СОХРАНЯЕМ ОТКРЫТУЮ ПОЗУ =====
            pointer1Start = pointer1.localRotation;
            pointer2Start = pointer2.localRotation;
            pointer3Start = pointer3.localRotation;

            middle1Start = middle1.localRotation;
            middle2Start = middle2.localRotation;
            middle3Start = middle3.localRotation;

            thumb1Start = thumb1.localRotation;
            thumb2Start = thumb2.localRotation;

            ring1Start = ring1.localRotation;
            ring2Start = ring2.localRotation;
            ring3Start = ring3.localRotation;

            pinky1Start = pinky1.localRotation;
            pinky2Start = pinky2.localRotation;
            pinky3Start = pinky3.localRotation;

            // ===== INDEX (только X) =====
            pointer1End = Quaternion.Euler(-50f, 0f, 0f);
            pointer2End = Quaternion.Euler(-90f, 0f, 0f);
            pointer3End = Quaternion.Euler(-70f, 0f, 0f);

            // ===== MIDDLE (только X) =====
            middle1End = Quaternion.Euler(-50f, 0f, 0f);
            middle2End = Quaternion.Euler(-90f, 0f, 0f);
            middle3End = Quaternion.Euler(-70f, 0f, 0f);

            // ===== THUMB (полный поворот) =====
            thumb1Start = Quaternion.Euler(0f, 0f, 55f);
            thumb2Start = Quaternion.Euler(0f, 0f, -12f);

            thumb1End = Quaternion.Euler(4f, -51f, 18f);
            thumb2End = Quaternion.Euler(-43f, 79f, -131f);

            // ===== RING (только X) =====
            ring1End = Quaternion.Euler(-50f, 0f, 0f);
            ring2End = Quaternion.Euler(-90f, 0f, 0f);
            ring3End = Quaternion.Euler(-70f, 0f, 0f);

            // ===== PINKY (только X) =====
            pinky1End = Quaternion.Euler(-50f, 0f, 0f);
            pinky2End = Quaternion.Euler(-90f, 0f, 0f);
            pinky3End = Quaternion.Euler(-70f, 0f, 0f);
        }


        void Update()
        {

            if (m_TriggerInput != null)
            {
                float t = m_TriggerInput.ReadValue();

                // ===== INDEX =====
                pointer1.localRotation = Quaternion.Slerp(pointer1Start, pointer1End, t);
                pointer2.localRotation = Quaternion.Slerp(pointer2Start, pointer2End, t);
                pointer3.localRotation = Quaternion.Slerp(pointer3Start, pointer3End, t);

                // ===== MIDDLE =====
                middle1.localRotation = Quaternion.Slerp(middle1Start, middle1End, t);
                middle2.localRotation = Quaternion.Slerp(middle2Start, middle2End, t);
                middle3.localRotation = Quaternion.Slerp(middle3Start, middle3End, t);

                // ===== THUMB =====
                thumb1.localRotation = Quaternion.Slerp(thumb1Start, thumb1End, t);
                thumb2.localRotation = Quaternion.Slerp(thumb2Start, thumb2End, t);


                ring1.localRotation = Quaternion.Slerp(ring1Start, ring1End, t);
                ring2.localRotation = Quaternion.Slerp(ring2Start, ring2End, t);
                ring3.localRotation = Quaternion.Slerp(ring3Start, ring3End, t);

                pinky1.localRotation = Quaternion.Slerp(pinky1Start, pinky1End, t);
                pinky2.localRotation = Quaternion.Slerp(pinky2Start, pinky2End, t);
                pinky3.localRotation = Quaternion.Slerp(pinky3Start, pinky3End, t);
            }
            




            if (m_GripInput != null)
            {
                //var gripVal = m_GripInput.ReadValue();
                //var currentPos = ring1.localPosition;
                //ring1.localPosition = new Vector3(Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal), currentPos.y, currentPos.z);
                //var currentPos2 = ring2.localPosition;
                //ring2.localPosition = new Vector3(Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal), currentPos2.y, currentPos2.z);
                //var currentPos3 = ring3.localPosition;
                //ring3.localPosition = new Vector3(Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal), currentPos3.y, currentPos3.z);
            }
        }
    }
}
