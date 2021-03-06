﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Diagnostics;

using SiliconStudio.Core;
using SiliconStudio.Xenko.UI.Events;

namespace SiliconStudio.Xenko.UI.Controls
{
    /// <summary>
    /// Represents the base primitive for all the button-like controls
    /// </summary>
    [DebuggerDisplay("ButtonBase - Name={Name}")]
    public abstract class ButtonBase : ContentControl
    {
        /// <summary>
        /// The key to the ClickMode dependency property.
        /// </summary>
        public readonly static PropertyKey<ClickMode> ClickModePropertyKey = new PropertyKey<ClickMode>("ClickModeKey", typeof(ButtonBase), DefaultValueMetadata.Static(ClickMode.Release));
        
        static ButtonBase()
        {
            EventManager.RegisterClassHandler(typeof(ButtonBase), ClickEvent, ClickClassHandler);
        }

        /// <summary>
        /// Create an instance of button.
        /// </summary>
        protected ButtonBase()
        {
            CanBeHitByUser = true;
        }

        /// <summary>
        /// Gets or sets when the Click event occurs.
        /// </summary>
        public ClickMode ClickMode
        {
            get { return DependencyProperties.Get(ClickModePropertyKey); }
            set { DependencyProperties.Set(ClickModePropertyKey, value); }
        }

        /// <summary>
        /// Gets a value that indicates whether the button is currently down.
        /// </summary>
        public virtual bool IsPressed { get; protected set; }

        /// <summary>
        /// Occurs when a <see cref="Button"/> is clicked.
        /// </summary>
        /// <remarks>A click event is bubbling</remarks>
        public event EventHandler<RoutedEventArgs> Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Click"/> routed event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = EventManager.RegisterRoutedEvent<RoutedEventArgs>(
            "Click",
            RoutingStrategy.Bubble,
            typeof(Button));

        protected override void OnTouchDown(TouchEventArgs args)
        {
            base.OnTouchDown(args);

            IsPressed = true;

            if (ClickMode == ClickMode.Press)
                RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
        
        protected override void OnTouchUp(TouchEventArgs args)
        {
            base.OnTouchUp(args);

            if (IsPressed && ClickMode == ClickMode.Release)
                RaiseEvent(new RoutedEventArgs(ClickEvent));

            IsPressed = false;
        }

        protected override void OnTouchLeave(TouchEventArgs args)
        {
            base.OnTouchLeave(args);

            IsPressed = false;
        }

        /// <summary>
        /// The class handler of the event <see cref="Click"/>.
        /// This method can be overridden in inherited classes to perform actions common to all instances of a class.
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected virtual void OnClick(RoutedEventArgs args)
        {

        }
        
        private static void ClickClassHandler(object sender, RoutedEventArgs args)
        {
            var buttonBase = (ButtonBase)sender;

            buttonBase.OnClick(args);
        }
    }
}