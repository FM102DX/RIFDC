using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RIFDC;
using CommonFunctions;

namespace RIFDCComponents.ExcelTreeViewBasedObjectReaderComponent
{
    public partial class ExcelTreeViewBasedObjectReader : Form, IRIFDCCrudForm
    {
        public ExcelTreeViewBasedObjectReader()
        {
            InitializeComponent();
        }
        DataFormComponent excelObjectReaderDfc;

        StaticExcelFile targetFile;

        ControlValueSaver saver;

        public Lib.InterFormMessage startMsg { get; set; }

        IKeeper ObjectDataSource = null;

        private void ExcelTreeViewBasedObjectReader_Load(object sender, EventArgs e)
        {
            saver = new ControlValueSaver(RIFDC_App.workingDir, "treeExcelLoarer");
            saver.addSaverCtrl(tbTargetFileName);
            saver.addSaverCtrl(tbWorkSheetName);
            saver.addSaverCtrl(tbRangeBegin);
            saver.addSaverCtrl(tbRangeEnd);

            fn.openFileDialogConstructor fnc = new fn.openFileDialogConstructor(tbTargetFileName, btnOpenFile);
            saver.loadIt();


        }

        private void writeMyText(string text)
        {
            tbInfo.Text += text + Environment.NewLine;
        }

        private void btnLoadTargetFile_Click(object sender, EventArgs e)
        {
            tbInfo.Text = "";

            writeMyText("Начинаем парсинг эксель-файла");

            saver.saveIt();

            fn.FilePathAnalyzer fpa = new fn.FilePathAnalyzer(tbTargetFileName.Text);

            //тут еще надо проверить, валидный файл или нет, но в этом случае targetFile должен быть null

            targetFile = new StaticExcelFile(fpa.getFilePath, fpa.getFileName, tbWorkSheetName.Text, tbRangeBegin.Text, tbRangeEnd.Text, 0);

            IDataCluster cluster = startMsg.excelStaticDelegate(targetFile);

            IDataRoom dataRoom = new DataRoom();

            dataRoom.actualCluster = cluster;

            Lib.DbOperationResult r = dataRoom.connect();

            if (r.success)
            {
                writeMyText("Файл открыт ");
            }
            else
            {
                writeMyText($"Не получилось открыть файл, msg={r.msg}");
                return;
            }

            //теперь надо создать IKeeper из того, что приехало в startMsg
            ObjectDataSource = startMsg.targetKeeper;

            ObjectDataSource.dataRoom = dataRoom;

            writeMyText("Читаем элементы...");
            try
            {
                ObjectDataSource.readItems(); // это вот сюда должно приехать дерево
                                              // а чтобы оно сюда приехало деревом, надо чтобы оно прочиталось деревом, там, где присваиваются ID
                                              // а это где происходит??

                dataRoom.disconnect();
                ObjectDataSource.arrangeTreeLevels();
                ObjectDataSource.dataRoom = null;
                writeMyText($"Прочитано объектов: {ObjectDataSource.count}");

            }
            catch (Exception ex)
            {
                writeMyText($"Ошибка: {ex}");
                writeMyText("Парсинг остановлен");
                return;
            }

            System.GC.Collect();

            //итак, что то он читает
            //надо бы в экселе посмотре
            //надо перезамапить грид

            writeMyText("Загружаем грид...");

            //DFC
            excelObjectReaderDfc = new DataFormComponent(ObjectDataSource, this, Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly, startMsg);

            //дерево
            RIFDC_TreeView tvr0 = new RIFDC_TreeView(tvrObjects, ObjectDataSource, startMsg.controlFormat);
            excelObjectReaderDfc.crudOperator.addGridBasedControlMapping(tvr0);

            //грид
            RIFDC_DataGridView grd1 = new RIFDC_DataGridView(dgObjects, ObjectDataSource, startMsg.controlFormat, DataGridEditabilityMode.NotEditableAtAll, true);
            excelObjectReaderDfc.crudOperator.addGridBasedControlMapping(grd1);

            //поля


            //кнопки
            excelObjectReaderDfc.crudOperator.addButtonMapping(new RIFDC_Button(btnToggleMultiSelect, FormBtnTypeEnum.btnToggleMultiSelectionMode));
            excelObjectReaderDfc.crudOperator.addButtonMapping(new RIFDC_Button(btnSelectAll, FormBtnTypeEnum.btnSelectAll));
            excelObjectReaderDfc.crudOperator.addButtonMapping(new RIFDC_Button(btnSelectNone, FormBtnTypeEnum.btnSelectNone));

            excelObjectReaderDfc.crudOperator.addButtonMapping(new RIFDC_Button(btnExpandAll, FormBtnTypeEnum.btnTreeViewExpandAll));
            excelObjectReaderDfc.crudOperator.addButtonMapping(new RIFDC_Button(btnCollapseAll, FormBtnTypeEnum.btnTreeViewCollapseAll));

            /*
            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnNext, FormBtnTypeEnum.btnRecordMoveNextRecord));
            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnPrev, FormBtnTypeEnum.btnRecordMovePrevRecord));
            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnAddNew, FormBtnTypeEnum.btnAddNew));
            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnDelete, FormBtnTypeEnum.btnDelete));
            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnSave, FormBtnTypeEnum.btnSaveRecord));

            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnSortUp, FormBtnTypeEnum.btnSortBySelectedFieldAsc));
            LoadedSpecLinesDFC.crudOperator.addButtonMapping(new RIFDC_Button(btnSortDown, FormBtnTypeEnum.btnSortBySelectedFieldDesc));
            */

            excelObjectReaderDfc.searchControl = new DFCSearchControl(excelObjectReaderDfc, Lib.FiltrationTypeEnum.Local, tbSearchField, btnGoSearch, btnResetSearch);

            //excelObjectReaderDfc.readItems();

            excelObjectReaderDfc.fillTheForm();

            writeMyText($"Парсинг завершен успешно");

            excelObjectReaderDfc.FormRefilled += ()=> {

                lbObjTypeText.Text = $"Тип объекта: {excelObjectReaderDfc.dataSource.sampleObject.entityType} количество объектов: {excelObjectReaderDfc.dataSource.actualItemList.Count}";

            };
        }



        private void ExcelTreeViewBasedObjectReader_FormClosing(object sender, FormClosingEventArgs e)
        {
            saver.saveIt();
        }

        private void btnPassItOn_Click(object sender, EventArgs e)
        {
            if (excelObjectReaderDfc == null) return;

            if (!(excelObjectReaderDfc.dataSource.count > 0)) return;


            // все выделенные объекты сохраняются в БД с входящим фильтром

            List<Lib.ObjectOperationResult> processionInfo = new List<Lib.ObjectOperationResult>();

            if (!fn.mb_confirmAction($"В основную БД будет внесено записей: { excelObjectReaderDfc.selectedItemsIds.Count} {fn.chr13}Продолжить?")) return;

            excelObjectReaderDfc.dataSource.dataRoom = startMsg.dataRoom; //NOTICE здесь как раз момент, когда мы в 1 кипере перелючаем датарумы

            if (excelObjectReaderDfc.multiSelectionMode)
            {
                excelObjectReaderDfc.selectedItemsIds.ForEach(x => {
                    processionInfo.Add(processImportingLine(x));
                });
            }
            else
            {
                processionInfo.Add(processImportingLine(excelObjectReaderDfc.dataSource.currentRecord.getMember().id));
            }

            int successCount = processionInfo.Where(x => x.success == true).ToList().Count;
            int unsuccessCount = processionInfo.Where(x => x.success == false).ToList().Count;
            string userMsg = $"Обработано {processionInfo.Count} записей {fn.chr13} Успешно {successCount} {fn.chr13} Ошибок: {unsuccessCount}";
            fn.mb_info(userMsg);
            excelObjectReaderDfc.fillTheForm();


        }

        private Lib.ObjectOperationResult processImportingLine(string id)
        {
            IKeepable line0 = excelObjectReaderDfc.dataSource.getItemById(id);

            if (line0 == null) return Lib.ObjectOperationResult.sayNo();

            IKeepable line = excelObjectReaderDfc.dataSource.createNewObject_inserted();

            // перебираем и устанавливаем в новый объект все параметры, которые прочитаны из экселя, т.е. у которых есть boundColumnNumber
            line.setMyParameter("parentId", line0.getMyParameter("parentId"));

            excelObjectReaderDfc
                .dataSource
                .sampleObject
                .fieldsInfo
                .fields.Where(x => x.excelFileBoundColumnNumber > 0)
                .ToList()
                .ForEach(y => {
                    line.setMyParameter(y.fieldClassName, line0.getMyParameter(y.fieldClassName));
                });

            return excelObjectReaderDfc.dataSource.saveItem(line);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {

        }
    }
}
