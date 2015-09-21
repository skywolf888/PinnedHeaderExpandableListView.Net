//package com.ryg.utils;

//import android.content.Context;
//import android.util.TypedValue;

using Android.Content;
using Android.Util;
using System;
namespace Com.Ryg.Utils
{
    public class Utils
    {

        public static int dp2px(Context context, int dp)
        {
            return (int)Math.Round(TypedValue.ApplyDimension(
                    ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics));
        }

    }
}