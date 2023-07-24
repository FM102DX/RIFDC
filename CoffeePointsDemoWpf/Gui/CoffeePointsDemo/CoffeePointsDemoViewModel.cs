using CoffeePointsDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace CoffeePointsDemoWpf
{
    public class CoffeePointsDemoViewModel : INotifyPropertyChanged
    {
        private Serilog.ILogger _logger;

        public SelectionMode SelectionModeVar { get; set; } = SelectionMode.None;

        public ICommand FormLoadedCmd { get; private set; }
        public ICommand SaveItemCmd { get; private set; }
        public ICommand CreateItemCmd { get; private set; }

        public ICommand DeleteItemCmd { get; private set; }
        
        public ICommand CancelRecordEditCmd { get; private set; }

        public event SelectionModeChangedDelegate SelectionModeChanged;

        public delegate void SelectionModeChangedDelegate(SelectionMode selectionMode);

        private bool IsModified
        {
            get
            {
                bool rez = !_itemManager.Similar(SelectedItemDisplayed, SelectedItem);
                return rez;
            }
        }

        private List<CoffeePoint> _activitiesList = new List<CoffeePoint>();

        public List<CoffeePoint> ItemListItemSource 
        { 
            get
            {
                return (List<CoffeePoint>) _activitiesList;    
            }
            
            set
            {
                _activitiesList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemListItemSource"));
            }
        }

        private CoffeePoint? _selectedItem;

        private CoffeePoint? _selectedItemDisplayed;

        //made to for record editing purposes
        public CoffeePoint? SelectedItemDisplayed 
        { 
            get
            {
                return _selectedItemDisplayed;
            }
            set
            {
                _selectedItemDisplayed=value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItemDisplayed"));
            }
        }

        public CoffeePoint? SelectedItem 
        {
            get 
            { 
                return _selectedItem; 
            }
            set
            {
                if (value == null && ItemListItemSource.Count != 0)
                {

                    return;
                }
                
                _selectedItem = value;

                if (_selectedItem == null)
                {
                    SelectionModeVar = SelectionMode.ActivityModeNoSelection;
                    UpdateSelectionMode();
                    SelectedItemDisplayed = null;
                    return;
                }

                SelectedItemDisplayed = _itemManager.Clone(_selectedItem);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));

                SelectionModeVar = SelectionMode.ActivityModeRegularSelection;

                UpdateSelectionMode();
            }
        }

        private TimeSpan _actStartTime { get; set; }

        private bool _saveActivityStopMarker = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public event Action NeedToCloseForm;

        public event Action NeedToCancelGridChange;

        public string BufferIn { get; set; }
        
        private CoffeePointsManager _itemManager;


        public CoffeePointsDemoViewModel (CoffeePointsManager itemManager, Serilog.ILogger logger)
        {
            _logger = logger;

            _itemManager = itemManager;

            SaveItemCmd = new ActionCommand(() =>
            {
                if (_selectedItem != null)
                {
                    SaveActivity();
                    LoadActivities(RecordSelectionMode.SelectSpecifiedId);
                }
            });



            CancelRecordEditCmd = new ActionCommand(() =>
            {
                if (SelectedItem !=null)
                {
                    SelectedItemDisplayed = _itemManager.Clone(SelectedItem);
                }
            });

            FormLoadedCmd = new ActionCommand(() =>
            {
              
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartDateTimeBindingVar"));

                UpdateSelectionMode();

            });

            CreateItemCmd = new ActionCommand(() =>
            {
               // add new activity to batch and re-read it into grid
               // get all activities for this batch

                // var activities = _itemManager.GetAll().Result.ToList();

                // LoadActivities(RecordSelectionMode.SelectSpecifiedId, newActivity.Id);

                //ShowFormSuccessMessage("Activity saved successfully");

            });

            DeleteItemCmd= new ActionCommand(() =>
            {
               /*
                * if (SelectedItem == null) return;
                var qRez = System.Windows.MessageBox.Show("Really delete?", "", MessageBoxButton.OKCancel);
                if (qRez == MessageBoxResult.Cancel) return;
                _itemManager.RemoveItem(SelectedItem.Id);
                LoadActivities( RecordSelectionMode.SelectLastRecord);
               */
            });

            LoadActivities(RecordSelectionMode.SelectFirstRecord);
        }
        public bool NeedToStopItemSelectionChange()
        {
            if (IsModified)
            {
                var mbxRez = MessageBox.Show("Save changes?", "Question", MessageBoxButton.YesNoCancel);

                if (mbxRez == MessageBoxResult.Yes)
                {
                    SaveActivity();

                    if (SelectedItemDisplayed!=null)
                    {
                        LoadActivities(RecordSelectionMode.SelectSpecifiedId);
                    }
                    else
                    {
                        LoadActivities();
                    }
                    return true;
                }
                
                if (mbxRez == MessageBoxResult.No)
                {
                    if (SelectedItem!=null) SelectedItemDisplayed =_itemManager.Clone(SelectedItem); else SelectedItemDisplayed = null;
                    return false;
                }

                if (mbxRez == MessageBoxResult.Cancel)
                {
                    return true;
                }

            }
            return false;
        }

        public bool NeedToStopFormClosing()
        {
            if (IsModified)
            {
                var mbxRez = MessageBox.Show("Save changes?", "Question", MessageBoxButton.YesNoCancel);

                if (mbxRez == MessageBoxResult.Yes)
                {
                    if (SaveActivity())
                    {
                        //saved successfully
                        return false;
                    }
                    else
                    {
                        if (SelectedItemDisplayed != null)
                        {
                            LoadActivities(RecordSelectionMode.SelectSpecifiedId);
                        }
                        else
                        {
                            LoadActivities();
                        }
                        return true;
                    }
                }

                if (mbxRez == MessageBoxResult.No)
                {
                    if (SelectedItem != null) SelectedItemDisplayed = _itemManager.Clone(SelectedItem); else SelectedItemDisplayed = null;
                    return false;
                }

                if (mbxRez == MessageBoxResult.Cancel)
                {
                    return true;
                }

            }
            return false;
        }

        private bool SaveActivity()
        {
            if (SelectedItemDisplayed == null) { return false; }
            var rez = _itemManager.ModifyItem(SelectedItemDisplayed).Result;
            if (!rez.Success)
            {
                ShowFormErrorMessage(rez.Message);
                return false;
            }
            ShowFormSuccessMessage("Activity saved successfully");
            return true;
        }
        private void ShowFormErrorMessage(string text)
        {
            MessageBox.Show(text,"Operation error", MessageBoxButton.OK , MessageBoxImage.Error);
        }

        private void ShowFormSuccessMessage(string text)
        {
            MessageBox.Show(text, "Operation successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadActivities(RecordSelectionMode recordSelectionMode= RecordSelectionMode.SelectNone, Guid? selectedItemId = null)
        {
            ItemListItemSource = _itemManager.GetAll().Result;
            
            if(ItemListItemSource.Count==0)
            {
                recordSelectionMode = RecordSelectionMode.SelectNone;
            }
            
            switch (recordSelectionMode)
            {
                case RecordSelectionMode.SelectNone:
                    SelectedItem = null;
                    break;

                case RecordSelectionMode.SelectFirstRecord:
                    if (ItemListItemSource.Count != 0)
                    {
                        SelectedItem = ItemListItemSource[0];
                    }
                    break;

                case RecordSelectionMode.SelectLastRecord:
                    if (ItemListItemSource.Count != 0)
                    {
                        SelectedItem = ItemListItemSource[ItemListItemSource.Count-1];
                    }
                    break;
                
                case RecordSelectionMode.SelectSpecifiedId:
                    if (ItemListItemSource.Count != 0 && selectedItemId !=null)
                    {
                        SelectedItem = ItemListItemSource.Where(x=>x.id== SelectedItem.id).ToList().FirstOrDefault();
                    }
                    break;

                case RecordSelectionMode.DontPerformSelection:
                    break;
            }
        }

        private void UpdateSelectionMode()
        {
            if (SelectionModeChanged != null)
            {
                SelectionModeChanged(SelectionModeVar);
            }
        }

        public enum SelectionMode
        {
            None=0,
            GroupMode = 1,
            ActivityModeNoSelection = 2,
            ActivityModeRegularSelection = 3
        }
        public enum SelectionMode2
        {
            DowSeen = 0,
            DowUnseen = 1
        }

        public enum RecordSelectionMode
        {
            SelectNone = 0,
            SelectFirstRecord = 1,
            SelectLastRecord = 2,
            SelectSpecifiedId = 3,
            DontPerformSelection=4
        }

    }


}
