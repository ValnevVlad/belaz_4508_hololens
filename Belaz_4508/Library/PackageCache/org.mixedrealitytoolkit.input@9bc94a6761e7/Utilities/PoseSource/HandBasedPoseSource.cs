// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using System;
using MixedReality.Toolkit.Subsystems;
using UnityEngine;

namespace MixedReality.Toolkit.Input
{
    /// <summary>
    /// Helps define a pose source that's based on a specific handedness with access to the current <see cref="HandsAggregatorSubsystem"/>.
    /// </summary>
    public abstract class HandBasedPoseSource : IPoseSource
    {
        [SerializeField]
        [Tooltip("The hand on which to track the joint.")]
        private Handedness hand;

        /// <summary>
        /// The hand to use for this pose source.
        /// </summary>
        public Handedness Hand { get => hand; set => hand = value; }

        /// <inheritdoc/>
        public abstract bool TryGetPose(out Pose pose);
    }
}
