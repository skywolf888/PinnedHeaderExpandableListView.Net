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

//import java.util.NoSuchElementException;

//import android.annotation.TargetApi;
//import android.content.Context;
//import android.os.Build;
//import android.util.AttributeSet;
//import android.util.Log;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.ViewConfiguration;
//import android.widget.LinearLayout;



using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
namespace Com.Ryg.Expandable.UI
{
    public interface OnGiveUpTouchEventListener
    {
        bool giveUpTouchEvent(MotionEvent mevent);
    }
    public class StickyLayout : LinearLayout
    {
        private const string TAG = "StickyLayout";
        private const bool DEBUG = true;

        

        private View mHeader;
        private View mContent;
        private OnGiveUpTouchEventListener mGiveUpTouchEventListener;

        // header的高度  单位：px
        private int mOriginalHeaderHeight;
        private int mHeaderHeight;

        private int mStatus = STATUS_EXPANDED;
        public const int STATUS_EXPANDED = 1;
        public const int STATUS_COLLAPSED = 2;

        private int mTouchSlop;

        // 分别记录上次滑动的坐标
        private int mLastX = 0;
        private int mLastY = 0;

        // 分别记录上次滑动的坐标(onInterceptTouchEvent)
        private int mLastXIntercept = 0;
        private int mLastYIntercept = 0;

        // 用来控制滑动角度，仅当角度a满足如下条件才进行滑动：tan a = deltaX / deltaY > 2
        private const int TAN = 2;

        private bool mIsSticky = true;
        private bool mInitDataSucceed = false;
        private bool mDisallowInterceptTouchEventOnHeader = true;

        public StickyLayout(Context context)
            : base(context)
        {

        }

        public StickyLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }

        //@TargetApi(Build.VERSION_CODES.HONEYCOMB)
        public StickyLayout(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }

        //@Override
        public override void OnWindowFocusChanged(bool hasWindowFocus)
        {
            base.OnWindowFocusChanged(hasWindowFocus);
            if (hasWindowFocus && (mHeader == null || mContent == null))
            {
                initData();
            }
        }

        private void initData()
        {
            int headerId = this.Resources.GetIdentifier("sticky_header", "id", this.Context.PackageName);
            int contentId = this.Resources.GetIdentifier("sticky_content", "id", this.Context.PackageName);
            if (headerId != 0 && contentId != 0)
            {
                mHeader = FindViewById(headerId);
                mContent = FindViewById(contentId);
                mOriginalHeaderHeight = mHeader.MeasuredHeight;
                mHeaderHeight = mOriginalHeaderHeight;
                mTouchSlop = ViewConfiguration.Get(this.Context).ScaledTouchSlop;
                if (mHeaderHeight > 0)
                {
                    mInitDataSucceed = true;
                }
                if (DEBUG)
                {
                    Log.Debug(TAG, "mTouchSlop = " + mTouchSlop + "mHeaderHeight = " + mHeaderHeight);
                }
            }
            else
            {
                throw new NoSuchElementException("Did your view with id \"sticky_header\" or \"sticky_content\" exists?");
            }
        }

        public void setOnGiveUpTouchEventListener(OnGiveUpTouchEventListener l)
        {
            mGiveUpTouchEventListener = l;
        }

        //@Override
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            int intercepted = 0;
            int x = (int)ev.GetX();
            int y = (int)ev.GetY();

            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    {
                        mLastXIntercept = x;
                        mLastYIntercept = y;
                        mLastX = x;
                        mLastY = y;
                        intercepted = 0;
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        int deltaX = x - mLastXIntercept;
                        int deltaY = y - mLastYIntercept;
                        if (mDisallowInterceptTouchEventOnHeader && y <= getHeaderHeight())
                        {
                            intercepted = 0;
                        }
                        else if (Math.Abs(deltaY) <= Math.Abs(deltaX))
                        {
                            intercepted = 0;
                        }
                        else if (mStatus == STATUS_EXPANDED && deltaY <= -mTouchSlop)
                        {
                            intercepted = 1;
                        }
                        else if (mGiveUpTouchEventListener != null)
                        {
                            if (mGiveUpTouchEventListener.giveUpTouchEvent(ev) && deltaY >= mTouchSlop)
                            {
                                intercepted = 1;
                            }
                        }
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        intercepted = 0;
                        mLastXIntercept = mLastYIntercept = 0;
                        break;
                    }
                default:
                    break;
            }

            if (DEBUG)
            {
                Log.Debug(TAG, "intercepted=" + intercepted);
            }
            return intercepted != 0 && mIsSticky;
        }

        //@Override
        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (!mIsSticky)
            {
                return true;
            }
            int x = (int)ev.GetX();
            int y = (int)ev.GetY();
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    {
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        int deltaX = x - mLastX;
                        int deltaY = y - mLastY;
                        if (DEBUG)
                        {
                            Log.Debug(TAG, "mHeaderHeight=" + mHeaderHeight + "  deltaY=" + deltaY + "  mlastY=" + mLastY);
                        }
                        mHeaderHeight += deltaY;
                        setHeaderHeight(mHeaderHeight);
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        // 这里做了下判断，当松开手的时候，会自动向两边滑动，具体向哪边滑，要看当前所处的位置
                        int destHeight = 0;
                        if (mHeaderHeight <= mOriginalHeaderHeight * 0.5)
                        {
                            destHeight = 0;
                            mStatus = STATUS_COLLAPSED;
                        }
                        else
                        {
                            destHeight = mOriginalHeaderHeight;
                            mStatus = STATUS_EXPANDED;
                        }
                        // 慢慢滑向终点
                        this.smoothSetHeaderHeight(mHeaderHeight, destHeight, 500);
                        break;
                    }
                default:
                    break;
            }
            mLastX = x;
            mLastY = y;
            return true;
        }

        public void smoothSetHeaderHeight(int from, int to, long duration)
        {
            smoothSetHeaderHeight(from, to, duration, false);
        }

        public void smoothSetHeaderHeight(int from, int to, long duration, bool modifyOriginalHeaderHeight)
        {
            //final int frameCount = (int) (duration / 1000f * 30) + 1;
            //final float partation = (to - from) / (float) frameCount;
            //new Thread("Thread#smoothSetHeaderHeight") {

            //    @Override
            //    public void run() {
            //        for (int i = 0; i < frameCount; i++) {
            //            final int height;
            //            if (i == frameCount - 1) {
            //                height = to;
            //            } else {
            //                height = (int) (from + partation * i);
            //            }
            //            post(new Runnable() {
            //                public void run() {
            //                    setHeaderHeight(height);
            //                }
            //            });
            //            try {
            //                sleep(10);
            //            } catch (InterruptedException e) {
            //                e.printStackTrace();
            //            }
            //        }

            //        if (modifyOriginalHeaderHeight) {
            //            setOriginalHeaderHeight(to);
            //        }
            //    };

            //}.start();
        }

        public void setOriginalHeaderHeight(int originalHeaderHeight)
        {
            mOriginalHeaderHeight = originalHeaderHeight;
        }

        public void setHeaderHeight(int height, bool modifyOriginalHeaderHeight)
        {
            if (modifyOriginalHeaderHeight)
            {
                setOriginalHeaderHeight(height);
            }
            setHeaderHeight(height);
        }

        public void setHeaderHeight(int height)
        {
            if (!mInitDataSucceed)
            {
                initData();
            }

            if (DEBUG)
            {
                Log.Debug(TAG, "setHeaderHeight height=" + height);
            }
            if (height <= 0)
            {
                height = 0;
            }
            else if (height > mOriginalHeaderHeight)
            {
                height = mOriginalHeaderHeight;
            }

            if (height == 0)
            {
                mStatus = STATUS_COLLAPSED;
            }
            else
            {
                mStatus = STATUS_EXPANDED;
            }

            if (mHeader != null && mHeader.LayoutParameters != null)
            {
                mHeader.LayoutParameters.Height = height;
                mHeader.RequestLayout();
                mHeaderHeight = height;
            }
            else
            {
                if (DEBUG)
                {
                    Log.Error(TAG, "null LayoutParams when setHeaderHeight");
                }
            }
        }

        public int getHeaderHeight()
        {
            return mHeaderHeight;
        }

        public void setSticky(bool isSticky)
        {
            mIsSticky = isSticky;
        }

        public void requestDisallowInterceptTouchEventOnHeader(bool disallowIntercept)
        {
            mDisallowInterceptTouchEventOnHeader = disallowIntercept;
        }

    }
}