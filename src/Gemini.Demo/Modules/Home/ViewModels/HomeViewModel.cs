﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Gemini.Framework;

namespace Gemini.Demo.Modules.Home.ViewModels
{
    [DisplayName("Home View Model")]
	[Export(typeof(HomeViewModel))]
	public class HomeViewModel : Document
	{
		private Color _background;
		public Color Background
		{
			get { return _background; }
			set
			{
				_background = value;
				NotifyOfPropertyChange(() => Background);
			}
		}

		private Color _foreground;
		public Color Foreground
		{
			get { return _foreground; }
			set
			{
				_foreground = value;
				NotifyOfPropertyChange(() => Foreground);
			}
		}

        private TextAlignment _textAlignment;
        [DisplayName("Text Alignment")]
        public TextAlignment TextAlignment
        {
            get { return _textAlignment; }
            set
            {
                _textAlignment = value;
                NotifyOfPropertyChange(() => TextAlignment);
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        private int _integerValue;
        [DisplayName("Integer Value")]
        public int IntegerValue
        {
            get { return _integerValue; }
            set
            {
                _integerValue = value;
                NotifyOfPropertyChange(() => IntegerValue);
            }
        }

        private int? _nullableIntegerValue;
        [DisplayName("Nullable Integer Value")]
        public int? NullableIntegerValue
        {
            get { return _nullableIntegerValue; }
            set
            {
                _nullableIntegerValue = value;
                NotifyOfPropertyChange(() => NullableIntegerValue);
            }
        }

        private float _floatValue;
        [DisplayName("Float Value")]
        public float FloatValue
        {
            get { return _floatValue; }
            set
            {
                _floatValue = value;
                NotifyOfPropertyChange(() => FloatValue);
            }
        }

        private float? _nullableFloatValue;
        [DisplayName("Nullable Float Value")]
        public float? NullableFloatValue
        {
            get { return _nullableFloatValue; }
            set
            {
                _nullableFloatValue = value;
                NotifyOfPropertyChange(() => NullableFloatValue);
            }
        }

        private double _doubleValue;
        [DisplayName("Double Value")]
        public double DoubleValue
        {
            get { return _doubleValue; }
            set
            {
                _doubleValue = value;
                NotifyOfPropertyChange(() => DoubleValue);
            }
        }

        private double? _nullableDoubleValue;
        [DisplayName("Nullable Double Value")]
        public double? NullableDoubleValue
        {
            get { return _nullableDoubleValue; }
            set
            {
                _nullableDoubleValue = value;
                NotifyOfPropertyChange(() => NullableDoubleValue);
            }
        }

        private TimeSpan? _timeOfDay;
        [DisplayName(@"Time Of Day")]
        public TimeSpan? TimeOfDay
        {
            get { return _timeOfDay; }
            set
            {
                if (value == _timeOfDay) return;
                _timeOfDay = value;
                NotifyOfPropertyChange(() => TimeOfDay);
            }
        }

        public HomeViewModel()
		{
		    DisplayName = "Home";
			Background = Colors.CornflowerBlue;
			Foreground = Colors.White;
		    TextAlignment = TextAlignment.Center;
		    Text = "Welcome to the Gemini Demo!";
		    IntegerValue = 3;
		    NullableIntegerValue = null;
		    FloatValue = 5.2f;
		    NullableFloatValue = null;
		    DoubleValue = Math.PI;
		    NullableDoubleValue = 4.5;
            TimeOfDay = DateTime.UtcNow.TimeOfDay;
		}
	}
}