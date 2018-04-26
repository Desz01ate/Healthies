using System;
using System.Collections.Generic;
using System.Linq;
using Android.Database;
using Android.Views;
using Android.Widget;
using Java.Lang;
using static Android.Support.V7.Widget.RecyclerView;

namespace HappyHealthyCSharp
{
    public class ViewHolder : Java.Lang.Object
    {
        public ImageView Picture { get; set; }
        public TextView Date { get; set; }
    }
    internal class CAdapter : BaseAdapter
    {
        private List<string> _textList;
        private List<bool> _stateList;

        public override int Count => _textList.Count();
        public CAdapter(List<string> mainText,List<bool> state)
        {
            _textList = mainText;
            _stateList = state;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            if(view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_diabetes, parent, false);
                var pic = view.FindViewById<ImageView>(Resource.Id.imageView2);
                var text = view.FindViewById<TextView>(Resource.Id.date);
                view.Tag = new ViewHolder() { Picture = pic,Date = text};
            }
            var holder = (ViewHolder)view.Tag;
            holder.Picture.SetImageResource(_stateList[position]?Resource.Drawable.ok:Resource.Drawable.danger);
            holder.Date.Text = _textList[position];
            return view;
        }
    }
}