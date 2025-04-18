using System.Collections;
using UnityEngine;
using UHFPS.Tools;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UHFPS.Input;

namespace UHFPS.Runtime
{
    public class LighterItem : PlayerItemBehaviour
    {
        public Light FlameLight;
        public Light SparkLight;
        public ParticleSystem SparkParticle;
        public MeshRenderer FlameRenderer;

        public float SparkLightTime;
        [Range(0f, 1f)] public float FlameIgniteProbability = 0.5f;
        [Range(0f, 1f)] public float FlameExtinguishProbability = 0.5f;
        public MinMax FlameExtinguishTimeRange;
        public bool EnableFlameExtinguishing;

        public MinMax FlameFlickerLimits;
        public float FlameFlickerSpeed;
        public float FlameLightIntensity = 1f;

        public string LighterDrawState = "LighterDraw";
        public string LighterHideState = "LighterHide";
        public string LighterIgniteStartState = "LighterIgniteStart";
        public string LighterIgniteSparkState = "LighterIgniteSpark";
        public string LighterIgniteHoldState = "LighterIgniteHold";

        public string LighterHideTrigger = "Hide";
        public string LighterSparkTrigger = "Spark";
        public string LighterHoldTrigger = "Hold";

        public SoundClip LighterFlick;
        private AudioSource audioSource;

        private bool flameIgnited;
        private bool isEquipped;
        private bool isBusy;

        private float flameExtinguishTime;
        private float sparkTime;

        public override string Name => "Lighter";

        public override bool IsBusy() => !isEquipped || isBusy;


        public float FlameLifetime = 2f;
        private Coroutine extinguishCoroutine;

        public ForTrigger trigger;

        public bool FirstEquip;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            trigger = GetComponent<ForTrigger>();
            FirstEquip = true;
        }

        public override void OnUpdate()
        {
            if (isEquipped)
            {
                if (FirstEquip)
                {
                    foreach (var wall in trigger.InvisibleWall)
                    {
                        wall.gameObject.SetActive(false);
                    }

                    FirstEquip = false;
                }

                if (flameIgnited)
                {
                    float flicker = Mathf.PerlinNoise(Time.time * FlameFlickerSpeed, 0);
                    float intensity = Mathf.Lerp(FlameFlickerLimits.RealMin, FlameFlickerLimits.RealMax, flicker) * FlameLightIntensity;
                    FlameLight.SetIntensity(intensity);
                    trigger.Istrigger = false;

                    //if (EnableFlameExtinguishing)
                    //{
                        //if ((flameExtinguishTime -= Time.deltaTime) <= 0)
                        //{
                            //if (PickTrueWithProbability(FlameExtinguishProbability))
                            //{
                                //FlameRenderer.gameObject.SetActive(false);
                                //Animator.SetBool(LighterHoldTrigger, false);
                                //StopAllCoroutines();
                                //StartCoroutine(FlameExtinguished());
                                //flameIgnited = false;
                            //}

                            //flameExtinguishTime = FlameExtinguishTimeRange.Random();
                        //}
                    //}
                }

                if (!flameIgnited)
                {
                    if (Keyboard.current.fKey.wasPressedThisFrame)
                    {
                        trigger.Istrigger = false;
                        StopAllCoroutines();
                        StartCoroutine(IgniteLighter(false));
                    }
                }

                if (sparkTime > 0) sparkTime -= Time.deltaTime;
                else SparkLight.enabled = false;
            }
        }

        public override void OnItemSelect()
        {
            ItemObject.SetActive(true);
            StartCoroutine(ShowLighter());
            flameExtinguishTime = FlameExtinguishTimeRange.Random();
            isEquipped = false;
        }

        IEnumerator ShowLighter()
        {
            yield return new WaitForAnimatorClip(Animator, LighterDrawState);
            isEquipped = true;

            yield return IgniteLighter(true);
        }

        IEnumerator FlameExtinguished()
        {
            yield return new WaitForSeconds(1f);
            yield return IgniteLighter(false);
        }

        IEnumerator IgniteLighter(bool isStart)
        {
            bool isIgnited = true;
            if (isStart)
            {
                Animator.SetBool(LighterHoldTrigger, true);
                Animator.SetBool(LighterSparkTrigger, false);
                yield return new WaitForAnimatorClip(Animator, LighterIgniteStartState);
            }   
            else
            {
                Animator.SetBool(LighterSparkTrigger, true);
                Animator.SetBool(LighterHoldTrigger, true);
                yield return new WaitForAnimatorClip(Animator, LighterIgniteSparkState);
            }

            FlameRenderer.gameObject.SetActive(true);
            Animator.SetBool(LighterHoldTrigger, true);
            Animator.SetBool(LighterSparkTrigger, false);
            flameIgnited = true;

            if (extinguishCoroutine != null) StopCoroutine(extinguishCoroutine);
            extinguishCoroutine = StartCoroutine(AutoExtinguish());
        }


        // IEnumerator IgniteLighter(bool isStart)
        // {
        //     bool isIgnited = PickTrueWithProbability(FlameIgniteProbability);

        //     if (isStart)
        //     {
        //         if (isIgnited)
        //         {
        //             Animator.SetBool(LighterHoldTrigger, true);
        //             Animator.SetBool(LighterSparkTrigger, false);
        //         }
        //         else Animator.SetBool(LighterSparkTrigger, true);
        //         yield return new WaitForAnimatorClip(Animator, LighterIgniteStartState);
        //     }
        //     else
        //     {
        //         Animator.SetBool(LighterSparkTrigger, true);
        //         if (isIgnited) Animator.SetBool(LighterHoldTrigger, true);
        //         yield return new WaitForAnimatorClip(Animator, LighterIgniteSparkState);
        //     }

        //     if (!isIgnited)
        //     {
        //         Animator.SetBool(LighterSparkTrigger, true);
        //         Animator.SetBool(LighterHoldTrigger, false);

        //         yield return new WaitForAnimatorStateEnter(Animator, LighterIgniteSparkState);

        //         do
        //         {
        //             if (isIgnited = PickTrueWithProbability(FlameIgniteProbability))
        //             {
        //                 Animator.SetBool(LighterSparkTrigger, false);
        //                 Animator.SetBool(LighterHoldTrigger, true);
        //             }

        //             yield return new WaitForAnimatorClip(Animator, LighterIgniteSparkState);
        //         }
        //         while (!isIgnited);
        //     }

        //     FlameRenderer.gameObject.SetActive(true);
        //     Animator.SetBool(LighterHoldTrigger, true);
        //     Animator.SetBool(LighterSparkTrigger, false);
        //     flameIgnited = true;
        // }

        IEnumerator AutoExtinguish()
        {
            yield return new WaitForSeconds(FlameLifetime);

            // ดับไฟแช็ก
            FlameRenderer.gameObject.SetActive(false);
            Animator.SetBool(LighterHoldTrigger, false);
            flameIgnited = false;
            trigger.Istrigger = true;
        }

        public void ShowSpark()
        {
            if (audioSource) audioSource.PlayOneShotSoundClip(LighterFlick);
            SparkParticle.Play();
            SparkLight.enabled = true;
            sparkTime = SparkLightTime;
        }

        public override void OnItemDeselect()
        {
            StopAllCoroutines();
            StartCoroutine(HideLighter());
            Animator.SetBool(LighterHoldTrigger, false);
            flameIgnited = false;
            isBusy = true;
        }

        IEnumerator HideLighter()
        {
            Animator.SetTrigger(LighterHideTrigger);
            FlameRenderer.gameObject.SetActive(false);
            SparkLight.enabled = true;

            yield return new WaitForAnimatorClip(Animator, LighterHideState);
            ItemObject.SetActive(false);

            sparkTime = 0;
            isEquipped = false;
            isBusy = false;
        }

        public override void OnItemActivate()
        {
            trigger.Istrigger = false;
            ItemObject.SetActive(true);
            Animator.Play(LighterIgniteHoldState);

            FlameRenderer.gameObject.SetActive(true);
            Animator.SetBool(LighterHoldTrigger, true);
            flameExtinguishTime = FlameExtinguishTimeRange.Random();

            StopAllCoroutines();
            flameIgnited = true;
            isEquipped = true;
        }

        public override void OnItemDeactivate()
        {
            StopAllCoroutines();

            FlameRenderer.gameObject.SetActive(true);
            SparkLight.enabled = true;
            ItemObject.SetActive(false);

            sparkTime = 0;
            flameIgnited = false;
            isEquipped = false;
            isBusy = false;
        }

        private bool PickTrueWithProbability(double probability)
        {
            System.Random rand = new();
            double randValue = rand.NextDouble();
            return randValue < probability;
        }

    }
}