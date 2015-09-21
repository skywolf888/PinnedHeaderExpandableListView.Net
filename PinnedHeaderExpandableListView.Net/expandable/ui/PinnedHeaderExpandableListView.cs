/**
The MIT License (MIT)

Copyright (c) 2014 singwhatiwanna
https://github.com/singwhatiwanna
http://blog.csdn.net/singwhatiwanna

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//package com.ryg.expandable.ui;

//import android.content.Context;
//import android.graphics.Canvas;
//import android.graphics.Rect;
//import android.util.AttributeSet;
//import android.util.Log;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.AbsListView;
//import android.widget.ExpandableListView;
//import android.widget.AbsListView.OnScrollListener;


using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;



namespace Com.Ryg.Expandable.UI
{

    public interface OnHeaderUpdateListener
    {
        /**
         * 返回一个view对象即可
         * 注意：view必须要有LayoutParams
         */
        View getPinnedHeader();

        void updatePinnedHeader(View headerView, int firstVisibleGroupPos);
    }
    public class PinnedHeaderExpandableListView : ExpandableListView, AbsListView.IOnScrollListener
    {
        private const string TAG = "PinnedHeaderExpandableListView";
        private const bool DEBUG = true;



        private View mHeaderView;
        private int mHeaderWidth;
        private int mHeaderHeight;

        private View mTouchTarget;

        private IOnScrollListener mScrollListener;
        private OnHeaderUpdateListener mHeaderUpdateListener;

        private bool mActionDownHappened = false;
        protected bool mIsHeaderGroupClickable = true;


        public PinnedHeaderExpandableListView(Context context)
            : base(context)
        {

            initView();
        }

        public PinnedHeaderExpandableListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

            initView();
        }

        public PinnedHeaderExpandableListView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

            initView();
        }

        private void initView()
        {
            SetFadingEdgeLength(0);
            SetOnScrollListener(this);
        }

        ////@Override
        public override void SetOnScrollListener(IOnScrollListener l)
        {
            if (l != this)
            {
                mScrollListener = l;
            }
            else
            {
                mScrollListener = null;
            }
            base.SetOnScrollListener(this);
        }

        /**
         * 给group添加点击事件监听
         * @param onGroupClickListener 监听
         * @param isHeaderGroupClickable 表示header是否可点击<br/>
         * note : 当不想group可点击的时候，需要在OnGroupClickListener#onGroupClick中返回true，
         * 并将isHeaderGroupClickable设为false即可
         */
        public void setOnGroupClickListener(IOnGroupClickListener onGroupClickListener, bool isHeaderGroupClickable)
        {
            mIsHeaderGroupClickable = isHeaderGroupClickable;
            base.SetOnGroupClickListener(onGroupClickListener);
        }

        public void setOnHeaderUpdateListener(OnHeaderUpdateListener listener)
        {
            mHeaderUpdateListener = listener;
            if (listener == null)
            {
                mHeaderView = null;
                mHeaderWidth = mHeaderHeight = 0;
                return;
            }
            mHeaderView = listener.getPinnedHeader();
            int firstVisiblePos = FirstVisiblePosition;
            int firstVisibleGroupPos = GetPackedPositionGroup(GetExpandableListPosition(firstVisiblePos));
            listener.updatePinnedHeader(mHeaderView, firstVisibleGroupPos);
            RequestLayout();
            PostInvalidate();
        }

        //@Override
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (mHeaderView == null)
            {
                return;
            }
            MeasureChild(mHeaderView, widthMeasureSpec, heightMeasureSpec);
            mHeaderWidth = mHeaderView.MeasuredWidth;
            mHeaderHeight = mHeaderView.MeasuredHeight;
        }

        //@Override
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (mHeaderView == null)
            {
                return;
            }
            int delta = mHeaderView.Top;
            mHeaderView.Layout(0, delta, mHeaderWidth, mHeaderHeight + delta);
        }

        //@Override
        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);
            if (mHeaderView != null)
            {
                DrawChild(canvas, mHeaderView, DrawingTime);
            }
        }

        //@Override
        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            int x = (int)ev.GetX();
            int y = (int)ev.GetY();
            int pos = PointToPosition(x, y);
            if (mHeaderView != null && y >= mHeaderView.Top && y <= mHeaderView.Bottom)
            {
                if (ev.Action == MotionEventActions.Down)
                {
                    mTouchTarget = getTouchTarget(mHeaderView, x, y);
                    mActionDownHappened = true;
                }
                else if (ev.Action == MotionEventActions.Up)
                {
                    View touchTarget = getTouchTarget(mHeaderView, x, y);
                    if (touchTarget == mTouchTarget && mTouchTarget.Clickable)
                    {
                        mTouchTarget.PerformClick();
                        Invalidate(new Rect(0, 0, mHeaderWidth, mHeaderHeight));
                    }
                    else if (mIsHeaderGroupClickable)
                    {
                        int groupPosition = GetPackedPositionGroup(GetExpandableListPosition(pos));
                        if (groupPosition != AdapterView.InvalidPosition && mActionDownHappened)
                        {
                            if (IsGroupExpanded(groupPosition))
                            {
                                CollapseGroup(groupPosition);
                            }
                            else
                            {
                                ExpandGroup(groupPosition);
                            }
                        }
                    }
                    mActionDownHappened = false;
                }
                return true;
            }

            return base.DispatchTouchEvent(ev);
        }

        private View getTouchTarget(View view, int x, int y)
        {
            if (!(view is ViewGroup))
            {
                return view;
            }

            ViewGroup parent = (ViewGroup)view;
            int childrenCount = parent.ChildCount;
            bool customOrder = ChildrenDrawingOrderEnabled;
            View target = null;
            for (int i = childrenCount - 1; i >= 0; i--)
            {
                int childIndex = customOrder ? GetChildDrawingOrder(childrenCount, i) : i;
                View child = parent.GetChildAt(childIndex);
                if (isTouchPointInView(child, x, y))
                {
                    target = child;
                    break;
                }
            }
            if (target == null)
            {
                target = parent;
            }

            return target;
        }

        private bool isTouchPointInView(View view, int x, int y)
        {

            if (view.Clickable && y >= view.Top && y <= view.Bottom
                    && x >= view.Left && x <= view.Right)
            {
                return true;
            }
            return false;
        }

        public void requestRefreshHeader()
        {
            refreshHeader();
            Invalidate(new Rect(0, 0, mHeaderWidth, mHeaderHeight));
        }

        protected void refreshHeader()
        {
            if (mHeaderView == null)
            {
                return;
            }
            int firstVisiblePos = FirstVisiblePosition;
            int pos = firstVisiblePos + 1;
            int firstVisibleGroupPos = GetPackedPositionGroup(GetExpandableListPosition(firstVisiblePos));
            int group = GetPackedPositionGroup(GetExpandableListPosition(pos));
            if (DEBUG)
            {
                Log.Debug(TAG, "refreshHeader firstVisibleGroupPos=" + firstVisibleGroupPos);
            }

            if (group == firstVisibleGroupPos + 1)
            {
                View view = GetChildAt(1);
                if (view == null)
                {
                    Log.Warn(TAG, "Warning : refreshHeader getChildAt(1)=null");
                    return;
                }
                if (view.Top <= mHeaderHeight)
                {
                    int delta = mHeaderHeight - view.Top;
                    mHeaderView.Layout(0, -delta, mHeaderWidth, mHeaderHeight - delta);
                }
                else
                {
                    //TODO : note it, when cause bug, remove it
                    mHeaderView.Layout(0, 0, mHeaderWidth, mHeaderHeight);
                }
            }
            else
            {
                mHeaderView.Layout(0, 0, mHeaderWidth, mHeaderHeight);
            }

            if (mHeaderUpdateListener != null)
            {
                mHeaderUpdateListener.updatePinnedHeader(mHeaderView, firstVisibleGroupPos);
            }
        }

        //@Override
        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            if (mScrollListener != null)
            {
                mScrollListener.OnScrollStateChanged(view, scrollState);
            }
        }

        //@Override
        public void OnScroll(AbsListView view, int firstVisibleItem,
                int visibleItemCount, int totalItemCount)
        {
            if (totalItemCount > 0)
            {
                refreshHeader();
            }
            if (mScrollListener != null)
            {
                mScrollListener.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);
            }
        }

    }
}