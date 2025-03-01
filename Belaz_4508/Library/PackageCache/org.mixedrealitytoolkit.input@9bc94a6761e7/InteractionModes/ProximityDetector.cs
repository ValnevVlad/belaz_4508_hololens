// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace MixedReality.Toolkit.Input
{
    /// <summary>
    /// An <see cref="IInteractionModeDetector"/> that detects nearby interactables.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Proximity Detector")]
    public class ProximityDetector : MonoBehaviour, IInteractionModeDetector
    {
        [SerializeField]
        [Tooltip("The interaction manager used to check the presence of interactables on colliders.")]
        private XRInteractionManager interactionManager;

        [SerializeField]
        [Tooltip("The interaction mode to be toggled if when the detector determines it to be active")]
        private InteractionMode modeOnDetection;

        /// <inheritdoc />
        public InteractionMode ModeOnDetection => modeOnDetection;

        /// <inheritdoc />
        /// <remarks>
        /// This is safe to read at any time, even during FixedUpdate. However, as a result of
        /// its internal buffering, it will return true for one extra frame after all objects
        /// have left the detection zone.
        /// </remarks>
        public virtual bool IsModeDetected() => detectedAnythingLastFrame || DetectedAnythingSoFar;

        [SerializeField]
        [FormerlySerializedAs("associatedControllers")]
        [FormerlySerializedAs("controllers")]
        [Tooltip("List of GameObjects which represent the interactor groups that this interaction mode detector has jurisdiction over. Interaction modes will be set on all specified groups.")]
        private List<GameObject> interactorGroups;

        /// <inheritdoc /> 
        [Obsolete("This function is obsolete and will be removed in a future version. Please use GetInteractorGroups instead.")]
        public List<GameObject> GetControllers() => GetInteractorGroups();

        /// <inheritdoc /> 
        public List<GameObject> GetInteractorGroups() => interactorGroups;

        // Visualizing the proximity zone
        private SphereCollider detectionZone;

        // Was an interactable detected within the proximity zone last physics-tick?
        private bool detectedAnythingLastFrame = false;

        // During this current FixedUpdate cycle, have we yet detected an interactable?
        private bool DetectedAnythingSoFar => colliders.Count > 0;

        // Hashset of colliders with IXRInteractables associated with them.
        private HashSet<Collider> colliders = new HashSet<Collider>();

        /// <summary>
        /// A collection of colliders currently detected by this proximity detector.
        /// Only colliders associated with IXRInteractables are included.
        /// </summary>
        public HashSet<Collider> DetectedColliders => colliders;

        /// <summary>
        /// The interaction manager to use to query interactors and their registration events.
        /// Currently protected internal, may be exposed in a future update.
        /// </summary>
        internal protected XRInteractionManager InteractionManager
        {
            get
            {
                if (interactionManager == null)
                {
                    interactionManager = ComponentCache<XRInteractionManager>.FindFirstActiveInstance();
                }

                return interactionManager;
            }
            set => interactionManager = value;
        }

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (interactionManager == null)
            {
                interactionManager = ComponentCache<XRInteractionManager>.FindFirstActiveInstance();
            }

            if (interactionManager == null)
            {
                Debug.LogWarning("No interaction manager found in scene. Please add an interaction manager to the scene.");
            }

            detectionZone = GetComponentInChildren<SphereCollider>();
        }

        private void OnTriggerStay(Collider other)
        {
            // Does this collider have an interactable associated with it?
            // We only detect actual interactables, not just all colliders.
            if (interactionManager != null && interactionManager.TryGetInteractableForCollider(other, out _))
            {
                colliders.Add(other);
            }
        }

        /// <summary>
        /// A Unity event function that is called at an framerate independent frequency, and is only called if this object is enabled.
        /// </summary>
        private void FixedUpdate()
        {
            detectedAnythingLastFrame = colliders.Count > 0;

            // Wipe the collection so that nothing persists from frame-to-frame.
            colliders.Clear();
        }

        /// <summary>
        /// A Unity event function that is called to draw Unity editor gizmos that are also interactable and always drawn.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (detectionZone != null)
            {
                Gizmos.color = Color.green - Color.black * 0.8f;
                // Gizmos.DrawSphere(attachTransform.position, detectionZone.radius);
            }
        }
    }
}
