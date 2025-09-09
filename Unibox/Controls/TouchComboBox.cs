using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Unibox.Controls
{
    public class TouchComboBox : ComboBox
    {
        public static readonly DependencyProperty TouchScrollSpeedProperty =
            DependencyProperty.Register(nameof(TouchScrollSpeed), typeof(double), typeof(TouchComboBox), new PropertyMetadata(1.0));

        public static readonly DependencyProperty TouchScrollAccelerationProperty =
            DependencyProperty.Register(nameof(TouchScrollAcceleration), typeof(double), typeof(TouchComboBox), new PropertyMetadata(0.1));

        public static readonly DependencyProperty TouchScrollDecelerationProperty =
            DependencyProperty.Register(nameof(TouchScrollDeceleration), typeof(double), typeof(TouchComboBox), new PropertyMetadata(0.1));

        public static readonly DependencyProperty MaxDropDownItemsProperty =
            DependencyProperty.Register(nameof(MaxDropDownItems), typeof(int), typeof(TouchComboBox), new PropertyMetadata(0, OnMaxDropDownItemsChanged));

        public static readonly DependencyProperty MaxDropDownHeightInPixelsProperty =
            DependencyProperty.Register(nameof(MaxDropDownHeightInPixels), typeof(double), typeof(TouchComboBox), new PropertyMetadata(0.0, OnMaxDropDownHeightInPixelsChanged));

        private ScrollViewer _scrollViewer;
        private bool _isTouchScrolling;
        private Point _lastTouchPoint;
        private double _velocity;
        private DispatcherTimer _inertiaTimer;
        private bool _preventSelection;

        static TouchComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchComboBox), new FrameworkPropertyMetadata(typeof(TouchComboBox)));
        }

        public double TouchScrollSpeed
        {
            get => (double)GetValue(TouchScrollSpeedProperty);
            set => SetValue(TouchScrollSpeedProperty, value);
        }

        public double TouchScrollAcceleration
        {
            get => (double)GetValue(TouchScrollAccelerationProperty);
            set => SetValue(TouchScrollAccelerationProperty, value);
        }

        public double TouchScrollDeceleration
        {
            get => (double)GetValue(TouchScrollDecelerationProperty);
            set => SetValue(TouchScrollDecelerationProperty, value);
        }

        public int MaxDropDownItems
        {
            get => (int)GetValue(MaxDropDownItemsProperty);
            set => SetValue(MaxDropDownItemsProperty, value);
        }

        public double MaxDropDownHeightInPixels
        {
            get => (double)GetValue(MaxDropDownHeightInPixelsProperty);
            set => SetValue(MaxDropDownHeightInPixelsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_inertiaTimer == null)
            {
                _inertiaTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
                _inertiaTimer.Tick += InertiaTimer_Tick;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.PreviewTouchDown -= ScrollViewer_PreviewTouchDown;
                _scrollViewer.PreviewTouchMove -= ScrollViewer_PreviewTouchMove;
                _scrollViewer.PreviewTouchUp -= ScrollViewer_PreviewTouchUp;
                _scrollViewer.PreviewMouseDown -= ScrollViewer_PreviewMouseDown;
            }

            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            if (_scrollViewer != null)
            {
                _scrollViewer.PreviewTouchDown += ScrollViewer_PreviewTouchDown;
                _scrollViewer.PreviewTouchMove += ScrollViewer_PreviewTouchMove;
                _scrollViewer.PreviewTouchUp += ScrollViewer_PreviewTouchUp;
                _scrollViewer.PreviewMouseDown += ScrollViewer_PreviewMouseDown;
            }

            UpdateDropDownHeight();
        }

        private void ScrollViewer_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            _isTouchScrolling = true;
            _lastTouchPoint = e.GetTouchPoint(_scrollViewer).Position;
            _velocity = 0;
            _inertiaTimer.Stop();
            _preventSelection = false;
            e.Handled = true;
        }

        private void ScrollViewer_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (_isTouchScrolling)
            {
                var currentPoint = e.GetTouchPoint(_scrollViewer).Position;
                double deltaY = _lastTouchPoint.Y - currentPoint.Y;
                _velocity = deltaY * TouchScrollSpeed;
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset + _velocity);
                _lastTouchPoint = currentPoint;
                _preventSelection = true;
                e.Handled = true;
            }
        }

        private void ScrollViewer_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (_isTouchScrolling)
            {
                _isTouchScrolling = false;
                _inertiaTimer.Start();
                e.Handled = true;
            }
        }

        private void InertiaTimer_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(_velocity) < 0.1)
            {
                _inertiaTimer.Stop();
                _velocity = 0;
                return;
            }
            _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset + _velocity);
            _velocity *= (1 - TouchScrollDeceleration);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Reset selection prevention for mouse
            _preventSelection = false;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (_preventSelection)
            {
                // Prevent selection if it was a scroll gesture
                e.Handled = true;
                return;
            }
            base.OnSelectionChanged(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            UpdateDropDownHeight();
        }

        private static void OnMaxDropDownItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TouchComboBox)d).UpdateDropDownHeight();
        }

        private static void OnMaxDropDownHeightInPixelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TouchComboBox)d).UpdateDropDownHeight();
        }

        private void UpdateDropDownHeight()
        {
            if (MaxDropDownHeightInPixels > 0)
            {
                this.MaxDropDownHeight = MaxDropDownHeightInPixels;
            }
            else if (MaxDropDownItems > 0 && Items.Count > 0)
            {
                var itemContainer = ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
                if (itemContainer != null)
                {
                    this.MaxDropDownHeight = itemContainer.ActualHeight * MaxDropDownItems;
                }
                else
                {
                    // Fallback: estimate item height
                    this.MaxDropDownHeight = 30 * MaxDropDownItems;
                }
            }
        }
    }
}