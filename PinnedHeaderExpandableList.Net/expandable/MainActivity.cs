//package com.ryg.expandable;

//import java.util.ArrayList;
//import java.util.List;

//import com.ryg.expandable.R;
//import com.ryg.expandable.ui.PinnedHeaderExpandableListView;
//import com.ryg.expandable.ui.StickyLayout;
//import com.ryg.expandable.ui.PinnedHeaderExpandableListView.OnHeaderUpdateListener;
//import com.ryg.expandable.ui.StickyLayout.OnGiveUpTouchEventListener;

//import android.app.Activity;
//import android.content.Context;
//import android.os.Bundle;
//import android.view.LayoutInflater;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.View.OnClickListener;
//import android.view.ViewGroup;
//import android.widget.AbsListView.LayoutParams;
//import android.widget.BaseExpandableListAdapter;
//import android.widget.Button;
//import android.widget.ExpandableListView;
//import android.widget.ImageView;
//import android.widget.TextView;
//import android.widget.Toast;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.ryg.expandable;
using Com.Ryg.Expandable.UI;
using System;
using System.Collections.Generic;
using R = PinnedHeaderExpandableList.Net.Resource;


namespace Com.Ryg.Expandable
{

    [Activity(Label = "ViewPagerIndicator.Net.Sample", MainLauncher = true, Icon = "@drawable/icon")]

    public class MainActivity : Activity,
            ExpandableListView.IOnChildClickListener,
            ExpandableListView.IOnGroupClickListener,
            OnHeaderUpdateListener, OnGiveUpTouchEventListener
    {
        private Com.Ryg.Expandable.UI.PinnedHeaderExpandableListView expandableListView;
        private StickyLayout stickyLayout;
        private static List<Group> groupList;
        private static List<List<People>> childList;

        private MyexpandableListAdapter adapter;

        ////@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.main);
            expandableListView = (Com.Ryg.Expandable.UI.PinnedHeaderExpandableListView)FindViewById(R.Id.expandablelist);
            stickyLayout = (StickyLayout)FindViewById(R.Id.sticky_layout);
            initData();

            adapter = new MyexpandableListAdapter(this);
            expandableListView.SetAdapter(adapter);

            // 展开所有group
            for (int i = 0, count = expandableListView.Count; i < count; i++)
            {
                expandableListView.ExpandGroup(i);
            }

            expandableListView.setOnHeaderUpdateListener(this);
            expandableListView.SetOnChildClickListener(this);
            expandableListView.setOnGroupClickListener(this,true);
            stickyLayout.setOnGiveUpTouchEventListener(this);

        }

        /***
         * InitData
         */
        void initData()
        {
            groupList = new List<Group>();
            Group group = null;
            for (int i = 0; i < 3; i++)
            {
                group = new Group();
                group.setTitle("group-" + i);
                groupList.Add(group);
            }

            childList = new List<List<People>>();
            for (int i = 0; i < groupList.Count; i++)
            {
                List<People> childTemp;
                if (i == 0)
                {
                    childTemp = new List<People>();
                    for (int j = 0; j < 13; j++)
                    {
                        People people = new People();
                        people.setName("yy-" + j);
                        people.setAge(30);
                        people.setAddress("sh-" + j);

                        childTemp.Add(people);
                    }
                }
                else if (i == 1)
                {
                    childTemp = new List<People>();
                    for (int j = 0; j < 8; j++)
                    {
                        People people = new People();
                        people.setName("ff-" + j);
                        people.setAge(40);
                        people.setAddress("sh-" + j);

                        childTemp.Add(people);
                    }
                }
                else
                {
                    childTemp = new List<People>();
                    for (int j = 0; j < 23; j++)
                    {
                        People people = new People();
                        people.setName("hh-" + j);
                        people.setAge(20);
                        people.setAddress("sh-" + j);

                        childTemp.Add(people);
                    }
                }
                childList.Add(childTemp);
            }

        }

        /***
         * 数据源
         * 
         * @author Administrator
         * 
         */
        class MyexpandableListAdapter : BaseExpandableListAdapter
        {
            private Context context;
            private LayoutInflater inflater;

            public MyexpandableListAdapter(Context context)
            {
                this.context = context;
                inflater = LayoutInflater.From(context);
            }

            // 返回父列表个数
            //@Override
            //public override int GetGroupCount()
            //{
                
            //}

            public override int GroupCount
            {
                get { return groupList.Count; }
            }

            // 返回子列表个数
            //@Override
            public override int GetChildrenCount(int groupPosition)
            {
                return childList[groupPosition].Count;
            }

            //@Override
            public override Java.Lang.Object GetGroup(int groupPosition)
            {

                return groupList[groupPosition];
            }

            public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
            {
                return childList[groupPosition][childPosition];
            }

            //@Override
            //public Object getChild(int groupPosition, int childPosition)
            //{
                
            //}

            //@Override
            public override long GetGroupId(int groupPosition)
            {
                return groupPosition;
            }

            //@Override
            public override long GetChildId(int groupPosition, int childPosition)
            {
                return childPosition;
            }

            //@Override
            //public override bool HasStableIds()
            //{

            //    return true;
            //}

            public override bool HasStableIds
            {
                get { return true; }
            }

            //@Override
            public override View GetGroupView(int groupPosition, bool isExpanded,
                    View convertView, ViewGroup parent)
            {
                GroupHolder groupHolder = null;
                if (convertView == null)
                {
                    groupHolder = new GroupHolder();
                    convertView = inflater.Inflate(R.Layout.group, null);
                    groupHolder.textView = (TextView)convertView
                            .FindViewById(R.Id.group);
                    groupHolder.imageView = (ImageView)convertView
                            .FindViewById(R.Id.image);
                    convertView.Tag=groupHolder;
                }
                else
                {
                    groupHolder = (GroupHolder)convertView.Tag;
                }

                groupHolder.textView.Text=((Group)GetGroup(groupPosition))
                        .getTitle();
                if (isExpanded)// ture is Expanded or false is not isExpanded
                    groupHolder.imageView.SetImageResource(R.Drawable.expanded);
                else
                    groupHolder.imageView.SetImageResource(R.Drawable.collapse);
                return convertView;
            }

            //@Override
            public override View GetChildView(int groupPosition, int childPosition,
                    bool isLastChild, View convertView, ViewGroup parent)
            {
                ChildHolder childHolder = null;
                if (convertView == null)
                {
                    childHolder = new ChildHolder();
                    convertView = inflater.Inflate(R.Layout.child, null);

                    childHolder.textName = (TextView)convertView
                            .FindViewById(R.Id.name);
                    childHolder.textAge = (TextView)convertView
                            .FindViewById(R.Id.age);
                    childHolder.textAddress = (TextView)convertView
                            .FindViewById(R.Id.address);
                    childHolder.imageView = (ImageView)convertView
                            .FindViewById(R.Id.image);
                    Button button = (Button)convertView
                            .FindViewById(R.Id.button1);
                    //throw new Exception("");
                    //button.setOnClickListener(new OnClickListener() {
                    //    //@Override
                    //    public void onClick(View v) {
                    //        Toast.makeText(MainActivity.this, "clicked pos=", Toast.LENGTH_SHORT).show();
                    //    }
                    //});
                    button.Click += delegate { 
                        Toast.MakeText(this.context, "clicked pos="+childPosition.ToString(), ToastLength.Short).Show();
                    };

                    convertView.Tag=childHolder;
                }
                else
                {
                    childHolder = (ChildHolder)convertView.Tag;
                }

                childHolder.textName.Text=((People)GetChild(groupPosition,
                        childPosition)).getName();
                childHolder.textAge.Text=((People)GetChild(
                        groupPosition, childPosition)).getAge().ToString();
                childHolder.textAddress.Text=((People)GetChild(groupPosition,
                        childPosition)).getAddress();

                return convertView;
            }

            //@Override
            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return true;
            }
        }

        //@Override
        public bool OnGroupClick(ExpandableListView parent, View v,
                int groupPosition, long id)
        {

            return false;

        }
               

        //@Override
        public bool OnChildClick(ExpandableListView parent, View v,
                int groupPosition, int childPosition, long id)
        {
            Toast.MakeText(this,
                    childList[groupPosition][childPosition].getName(), ToastLength.Long).Show();

            return false;
        }

        class GroupHolder:Java.Lang.Object
        {
            public TextView textView;
            public ImageView imageView;
        }

        class ChildHolder:Java.Lang.Object
        {
            public TextView textName;
            public TextView textAge;
            public TextView textAddress;
            public ImageView imageView;
        }

        //@Override
        public View getPinnedHeader()
        {
            View headerView = (ViewGroup)LayoutInflater.Inflate(R.Layout.group, null);
            headerView.LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            return headerView;
        }

        //@Override
        public void updatePinnedHeader(View headerView, int firstVisibleGroupPos)
        {
            Group firstVisibleGroup = (Group)adapter.GetGroup(firstVisibleGroupPos);
            TextView textView = (TextView)headerView.FindViewById(R.Id.group);
            textView.Text=firstVisibleGroup.getTitle();
        }

        //@Override
        public bool giveUpTouchEvent(MotionEvent mevent)
        {
            if (expandableListView.FirstVisiblePosition == 0)
            {
                View view = expandableListView.GetChildAt(0);
                if (view != null && view.Top >= 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}