﻿using StaticFieldEpidEval.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StaticFieldEpidEval.ViewModels.Base
{
    /// <summary>
    /// Base class for all ViewModels, implements INotifyPropertyChanged and provides a collection of Check objects
    /// </summary>
    internal class BaseViewModel
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Check> Checks { get; set; }
    }
}
