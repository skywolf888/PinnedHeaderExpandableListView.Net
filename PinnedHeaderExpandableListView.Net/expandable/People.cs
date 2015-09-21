//package com.ryg.expandable;
namespace Com.Ryg.Expandable
{
    public class People:Java.Lang.Object
    {

        private string name;
        private int age;
        private string address;

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public int getAge()
        {
            return age;
        }

        public void setAge(int age)
        {
            this.age = age;
        }

        public string getAddress()
        {
            return address;
        }

        public void setAddress(string address)
        {
            this.address = address;
        }

    }
}