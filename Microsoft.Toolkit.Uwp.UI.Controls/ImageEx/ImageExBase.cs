﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Shared Code for ImageEx and RoundImageEx
    /// </summary>
    [TemplateVisualState(Name = LoadingState, GroupName = CommonGroup)]
    [TemplateVisualState(Name = LoadedState, GroupName = CommonGroup)]
    [TemplateVisualState(Name = UnloadedState, GroupName = CommonGroup)]
    [TemplateVisualState(Name = FailedState, GroupName = CommonGroup)]
    [TemplatePart(Name = PartImage, Type = typeof(object))]
    [TemplatePart(Name = PartProgress, Type = typeof(ProgressRing))]
    public abstract partial class ImageExBase : Control
    {
        protected const string PartImage = "Image";
        protected const string PartProgress = "Progress";
        protected const string CommonGroup = "CommonStates";
        protected const string LoadingState = "Loading";
        protected const string LoadedState = "Loaded";
        protected const string UnloadedState = "Unloaded";
        protected const string FailedState = "Failed";

        protected object Image { get; private set; }

        protected ProgressRing Progress { get; private set; }

        protected bool IsInitialized { get; private set; }

        protected object LockObj { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageExBase"/> class.
        /// </summary>
        public ImageExBase()
        {
            LockObj = new object();
        }

        protected void AttachImageOpened(RoutedEventHandler handler)
        {
            var image = Image as Image;
            var brush = Image as ImageBrush;

            if (image != null)
            {
                image.ImageOpened += handler;
            }
            else if (brush != null)
            {
                brush.ImageOpened += handler;
            }
        }

        protected void RemoveImageOpened(RoutedEventHandler handler)
        {
            var image = Image as Image;
            var brush = Image as ImageBrush;

            if (image != null)
            {
                image.ImageOpened -= handler;
            }
            else if (brush != null)
            {
                brush.ImageOpened -= handler;
            }
        }

        protected void AttachImageFailed(ExceptionRoutedEventHandler handler)
        {
            var image = Image as Image;
            var brush = Image as ImageBrush;

            if (image != null)
            {
                image.ImageFailed += handler;
            }
            else if (brush != null)
            {
                brush.ImageFailed += handler;
            }
        }

        protected void RemoveImageFailed(ExceptionRoutedEventHandler handler)
        {
            var image = Image as Image;
            var brush = Image as ImageBrush;

            if (image != null)
            {
                image.ImageFailed -= handler;
            }
            else if (brush != null)
            {
                brush.ImageFailed -= handler;
            }
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            RemoveImageOpened(OnImageOpened);
            RemoveImageFailed(OnImageFailed);

            Image = GetTemplateChild(PartImage) as object;
            Progress = GetTemplateChild(PartProgress) as ProgressRing;

            IsInitialized = true;

            SetSource(Source);

            AttachImageOpened(OnImageOpened);
            AttachImageFailed(OnImageFailed);

            base.OnApplyTemplate();
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var newSquareSize = Math.Min(finalSize.Width, finalSize.Height) / 8.0;

            if (Progress?.Width == newSquareSize)
            {
                Progress.Height = newSquareSize;
            }

            return base.ArrangeOverride(finalSize);
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            ImageOpened?.Invoke(this, e);
            ImageExOpened?.Invoke(this, new ImageExOpenedEventArgs());
            VisualStateManager.GoToState(this, LoadedState, true);
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ImageFailed?.Invoke(this, e);
            ImageExFailed?.Invoke(this, new ImageExFailedEventArgs(new Exception(e.ErrorMessage)));
            VisualStateManager.GoToState(this, FailedState, true);
        }
    }
}