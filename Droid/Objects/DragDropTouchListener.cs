using AlwaysOn.Objects;
using Android.Views;
using System;

namespace AlwaysOn_Droid
{
    public class DragDropTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private static int MIN_CLICK_DURATION = 200;
        private long StartClickTime;

        public DragDropTouchListener()
        {
        }

        public event EventHandler<View> LongClicked;
        protected void OnLongClicked(object sender, View e)
        {
            if (LongClicked != null)
                LongClicked(sender, e);
        }

        public event EventHandler<DragDropTouchEventArgs> Click;
        protected void OnClick(object sender, DragDropTouchEventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }

        public event EventHandler<DragDropTouchEventArgs> Dragging;
        protected void OnDragging(object sender, DragDropTouchEventArgs e)
        {
            if (Dragging != null)
                Dragging(sender, e);
        }


        public event EventHandler<DragDropTouchEventArgs> Dropped;
        protected void OnDropped(object sender, DragDropTouchEventArgs e)
        {
            if (Dropped != null)
                Dropped(sender, e);
        }

        public bool ClickedLong { get; private set; } = false;
        public bool ClickedLongDone { get; set; } = false;
        public bool DidDrag { get; private set; }
        public TouchPoint DownAt { get; private set; } = TouchPoint.Empty;
        public TouchPoint DragAt { get; private set; } = TouchPoint.Empty;
        public TouchPoint ViewWasAt { get; private set; } = TouchPoint.Empty;
        public TouchPoint Delta
        {
            get { return new TouchPoint(DragAt.X - DownAt.X, DragAt.Y - DownAt.Y); }
        }
        public bool Active { get { return DidDrag; } }
        public DragDropTouchListenerState State { get; set; }
  
        public void Reset()
        {
            State = DragDropTouchListenerState.Possible;
            DownAt = TouchPoint.Empty;
            DragAt = TouchPoint.Empty;
            DidDrag = false;
            ClickedLong = false;
        }

        private float Distance(TouchPoint point1, TouchPoint point2)
        {
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public bool OnTouch(View view, MotionEvent me)
        {
            int X = (int)me.RawX;
            int Y = (int)me.RawY;
            switch (me.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    {
                        DownAt = new TouchPoint(X, Y);
                        DragAt = DownAt;
                        ViewWasAt = new TouchPoint(view.GetX(), view.GetY());
                        State = DragDropTouchListenerState.Possible;

                        StartClickTime = Java.Util.Calendar.Instance.TimeInMillis;
                        //OnHeld(this, new DragDropTouchEventArgs(default(DragDropTouchListenerState), DragAt, Delta, ViewWasAt, view));
                    }
                    break;
                case MotionEventActions.Up:
                    {
                        if (DidDrag || ClickedLongDone)
                        {
                            State = DragDropTouchListenerState.Recognized;
                            OnDropped(this, new DragDropTouchEventArgs(State, DragAt, Delta, ViewWasAt, view));
                        }
                        else
                            OnClick(this, new DragDropTouchEventArgs(default(DragDropTouchListenerState), DragAt, Delta, ViewWasAt, view));

                        Reset();

                        break;
                    }
                case MotionEventActions.PointerDown:
                    break;
                case MotionEventActions.PointerUp:
                    break;
                case MotionEventActions.Move:
                    {
                        if (!ClickedLong && Java.Util.Calendar.Instance.TimeInMillis - StartClickTime > MIN_CLICK_DURATION)
                        {
                            ClickedLong = true;
                            OnLongClicked(this, view);
                        }

                        if (ClickedLongDone)
                        {
                            DragAt = new TouchPoint(X, Y);

                            if (State == DragDropTouchListenerState.Failed)
                                break;

                            if (Distance(DownAt, DragAt) > 30 || DidDrag)
                            {
                                DidDrag = true;
                                State = DragDropTouchListenerState.Changed;
                                OnDragging(this, new DragDropTouchEventArgs(State, DragAt, Delta, ViewWasAt, view));
                            }
                        }

                        break;
                    }
                case MotionEventActions.Cancel:
                    {
                        State = DragDropTouchListenerState.Failed;
                        ClickedLong = false;
                        ClickedLongDone = false;

                        break;
                    }
            }

            return true;
        }
        
    }

    public class DragDropTouchEventArgs : EventArgs
    {
        public DragDropTouchEventArgs(DragDropTouchListenerState state, TouchPoint point, TouchPoint delta, TouchPoint viewWasAt, View view)
        {
            State = state;
            Point = point;
            Delta = delta;
            ViewWasAt = viewWasAt;
            View = view;
        }

        public DragDropTouchListenerState State { get; private set; }
        public TouchPoint Point { get; private set; }
        public TouchPoint Delta { get; private set; }
        public TouchPoint ViewWasAt { get; private set; }
        public View View { get; private set; }
    }

    public enum DragDropTouchListenerState
    {
        Possible = 0,
        Began = 1,
        Changed = 2,
        Ended = 3,
        Recognized = 3,
        Cancelled = 4,
        Failed = 5
    };

    public class TouchPoint : Java.Lang.Object
    {
        public static readonly TouchPoint Empty;
        public TouchPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }
    }
}