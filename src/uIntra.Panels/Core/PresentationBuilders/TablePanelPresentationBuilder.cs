﻿using uIntra.Panels.Core.Models.Table;
using Uintra.Core.Extensions;

namespace UIntra.Panels.Core.PresentationBuilders
{
    public class TablePanelPresentationBuilder : ITablePanelPresentationBuilder
    {
        private readonly ITableCellBuilder _tableCellBuilder;

        public TablePanelPresentationBuilder(ITableCellBuilder tableCellBuilder)
        {
            _tableCellBuilder = tableCellBuilder;
        }

        public TablePanelViewModel Get(TableEditorModel model)
        {
            var table = model.Map<TableEditorViewModel>();
            table.Cells = _tableCellBuilder.Map(model.Cells, model.MakeFirstColumnBold);

            var result = new TablePanelViewModel
            {
                Table = table
            };

            return result;
        }

    }
}