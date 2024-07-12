"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var config_1 = require("./config");
var common_1 = require("./common");
var models_1 = require("./models");
var documentHandler_1 = require("./documentHandler");
var polyfills_1 = require("./polyfills");
var autoTableText_1 = require("./autoTableText");
var tablePrinter_1 = require("./tablePrinter");
function drawTable(jsPDFDoc, table) {
    var settings = table.settings;
    var startY = settings.startY;
    var margin = settings.margin;
    var cursor = {
        x: margin.left,
        y: startY,
    };
    var sectionsHeight = table.getHeadHeight(table.columns) + table.getFootHeight(table.columns);
    var minTableBottomPos = startY + margin.bottom + sectionsHeight;
    if (settings.pageBreak === 'avoid') {
        var rows = table.allRows();
        var tableHeight = rows.reduce(function (acc, row) { return acc + row.height; }, 0);
        minTableBottomPos += tableHeight;
    }
    var doc = new documentHandler_1.DocHandler(jsPDFDoc);
    if (settings.pageBreak === 'always' ||
        (settings.startY != null && minTableBottomPos > doc.pageSize().height)) {
        nextPage(doc);
        cursor.y = margin.top;
    }
    var startPos = polyfills_1.assign({}, cursor);
    table.startPageNumber = doc.pageNumber();
    if (settings.horizontalPageBreak === true) {
        // managed flow for split columns
        printTableWithHorizontalPageBreak(doc, table, startPos, cursor);
    }
    else {
        // normal flow
        doc.applyStyles(doc.userStyles);
        if (settings.showHead === 'firstPage' ||
            settings.showHead === 'everyPage') {
            table.head.forEach(function (row) {
                return printRow(doc, table, row, cursor, table.columns);
            });
        }
        doc.applyStyles(doc.userStyles);
        table.body.forEach(function (row, index) {
            var isLastRow = index === table.body.length - 1;
            printFullRow(doc, table, row, isLastRow, startPos, cursor, table.columns);
        });
        doc.applyStyles(doc.userStyles);
        if (settings.showFoot === 'lastPage' || settings.showFoot === 'everyPage') {
            table.foot.forEach(function (row) {
                return printRow(doc, table, row, cursor, table.columns);
            });
        }
    }
    common_1.addTableBorder(doc, table, startPos, cursor);
    table.callEndPageHooks(doc, cursor);
    table.finalY = cursor.y;
    jsPDFDoc.lastAutoTable = table;
    jsPDFDoc.previousAutoTable = table; // Deprecated
    if (jsPDFDoc.autoTable)
        jsPDFDoc.autoTable.previous = table; // Deprecated
    doc.applyStyles(doc.userStyles);
}
exports.drawTable = drawTable;
function printTableWithHorizontalPageBreak(doc, table, startPos, cursor) {
    // calculate width of columns and render only those which can fit into page
    var allColumnsCanFitResult = tablePrinter_1.default.calculateAllColumnsCanFitInPage(doc, table);
    allColumnsCanFitResult.map(function (colsAndIndexes, index) {
        doc.applyStyles(doc.userStyles);
        // add page to print next columns in new page
        if (index > 0) {
            addPage(doc, table, startPos, cursor, colsAndIndexes.columns);
        }
        else {
            // print head for selected columns
            printHead(doc, table, cursor, colsAndIndexes.columns);
        }
        // print body for selected columns
        printBody(doc, table, startPos, cursor, colsAndIndexes.columns);
        // print foot for selected columns
        printFoot(doc, table, cursor, colsAndIndexes.columns);
    });
}
function printHead(doc, table, cursor, columns) {
    var settings = table.settings;
    doc.applyStyles(doc.userStyles);
    if (settings.showHead === 'firstPage' || settings.showHead === 'everyPage') {
        table.head.forEach(function (row) { return printRow(doc, table, row, cursor, columns); });
    }
}
function printBody(doc, table, startPos, cursor, columns) {
    doc.applyStyles(doc.userStyles);
    table.body.forEach(function (row, index) {
        var isLastRow = index === table.body.length - 1;
        printFullRow(doc, table, row, isLastRow, startPos, cursor, columns);
    });
}
function printFoot(doc, table, cursor, columns) {
    var settings = table.settings;
    doc.applyStyles(doc.userStyles);
    if (settings.showFoot === 'lastPage' || settings.showFoot === 'everyPage') {
        table.foot.forEach(function (row) { return printRow(doc, table, row, cursor, columns); });
    }
}
function getRemainingLineCount(cell, remainingPageSpace, doc) {
    var fontHeight = (cell.styles.fontSize / doc.scaleFactor()) * config_1.FONT_ROW_RATIO;
    var vPadding = cell.padding('vertical');
    var remainingLines = Math.floor((remainingPageSpace - vPadding) / fontHeight);
    return Math.max(0, remainingLines);
}
function modifyRowToFit(row, remainingPageSpace, table, doc) {
    var cells = {};
    row.spansMultiplePages = true;
    row.height = 0;
    var rowHeight = 0;
    for (var _i = 0, _a = table.columns; _i < _a.length; _i++) {
        var column = _a[_i];
        var cell = row.cells[column.index];
        if (!cell)
            continue;
        if (!Array.isArray(cell.text)) {
            cell.text = [cell.text];
        }
        var remainderCell = new models_1.Cell(cell.raw, cell.styles, cell.section);
        remainderCell = polyfills_1.assign(remainderCell, cell);
        remainderCell.text = [];
        var remainingLineCount = getRemainingLineCount(cell, remainingPageSpace, doc);
        if (cell.text.length > remainingLineCount) {
            remainderCell.text = cell.text.splice(remainingLineCount, cell.text.length);
        }
        var scaleFactor = doc.scaleFactor();
        cell.contentHeight = cell.getContentHeight(scaleFactor);
        if (cell.contentHeight >= remainingPageSpace) {
            cell.contentHeight = remainingPageSpace;
            remainderCell.styles.minCellHeight -= remainingPageSpace;
        }
        if (cell.contentHeight > row.height) {
            row.height = cell.contentHeight;
        }
        remainderCell.contentHeight = remainderCell.getContentHeight(scaleFactor);
        if (remainderCell.contentHeight > rowHeight) {
            rowHeight = remainderCell.contentHeight;
        }
        cells[column.index] = remainderCell;
    }
    var remainderRow = new models_1.Row(row.raw, -1, row.section, cells, true);
    remainderRow.height = rowHeight;
    for (var _b = 0, _c = table.columns; _b < _c.length; _b++) {
        var column = _c[_b];
        var remainderCell = remainderRow.cells[column.index];
        if (remainderCell) {
            remainderCell.height = remainderRow.height;
        }
        var cell = row.cells[column.index];
        if (cell) {
            cell.height = row.height;
        }
    }
    return remainderRow;
}
function shouldPrintOnCurrentPage(doc, row, remainingPageSpace, table) {
    var pageHeight = doc.pageSize().height;
    var margin = table.settings.margin;
    var marginHeight = margin.top + margin.bottom;
    var maxRowHeight = pageHeight - marginHeight;
    if (row.section === 'body') {
        // Should also take into account that head and foot is not
        // on every page with some settings
        maxRowHeight -=
            table.getHeadHeight(table.columns) + table.getFootHeight(table.columns);
    }
    var minRowHeight = row.getMinimumRowHeight(table.columns, doc);
    var minRowFits = minRowHeight < remainingPageSpace;
    if (minRowHeight > maxRowHeight) {
        console.error("Will not be able to print row " + row.index + " correctly since it's minimum height is larger than page height");
        return true;
    }
    if (!minRowFits) {
        return false;
    }
    var rowHasRowSpanCell = row.hasRowSpan(table.columns);
    var rowHigherThanPage = row.getMaxCellHeight(table.columns) > maxRowHeight;
    if (rowHigherThanPage) {
        if (rowHasRowSpanCell) {
            console.error("The content of row " + row.index + " will not be drawn correctly since drawing rows with a height larger than the page height and has cells with rowspans is not supported.");
        }
        return true;
    }
    if (rowHasRowSpanCell) {
        // Currently a new page is required whenever a rowspan row don't fit a page.
        return false;
    }
    if (table.settings.rowPageBreak === 'avoid') {
        return false;
    }
    // In all other cases print the row on current page
    return true;
}
function printFullRow(doc, table, row, isLastRow, startPos, cursor, columns) {
    var remainingSpace = getRemainingPageSpace(doc, table, isLastRow, cursor);
    if (row.canEntireRowFit(remainingSpace, columns)) {
        printRow(doc, table, row, cursor, columns);
    }
    else {
        if (shouldPrintOnCurrentPage(doc, row, remainingSpace, table)) {
            var remainderRow = modifyRowToFit(row, remainingSpace, table, doc);
            printRow(doc, table, row, cursor, columns);
            addPage(doc, table, startPos, cursor, columns);
            printFullRow(doc, table, remainderRow, isLastRow, startPos, cursor, columns);
        }
        else {
            addPage(doc, table, startPos, cursor, columns);
            printFullRow(doc, table, row, isLastRow, startPos, cursor, columns);
        }
    }
}
function printRow(doc, table, row, cursor, columns) {
    cursor.x = table.settings.margin.left;
    for (var _i = 0, columns_1 = columns; _i < columns_1.length; _i++) {
        var column = columns_1[_i];
        var cell = row.cells[column.index];
        if (!cell) {
            cursor.x += column.width;
            continue;
        }
        doc.applyStyles(cell.styles);
        cell.x = cursor.x;
        cell.y = cursor.y;
        var result = table.callCellHooks(doc, table.hooks.willDrawCell, cell, row, column, cursor);
        if (result === false) {
            cursor.x += column.width;
            continue;
        }
        drawCellBorders(doc, cell, cursor);
        var textPos = cell.getTextPos();
        autoTableText_1.default(cell.text, textPos.x, textPos.y, {
            halign: cell.styles.halign,
            valign: cell.styles.valign,
            maxWidth: Math.ceil(cell.width - cell.padding('left') - cell.padding('right')),
        }, doc.getDocument());
        table.callCellHooks(doc, table.hooks.didDrawCell, cell, row, column, cursor);
        cursor.x += column.width;
    }
    cursor.y += row.height;
}
function drawCellBorders(doc, cell, cursor) {
    var cellStyles = cell.styles;
    doc.getDocument().setFillColor(doc.getDocument().getFillColor());
    if (typeof cellStyles.lineWidth === 'number') {
        // prints normal cell border
        var fillStyle = common_1.getFillStyle(cellStyles.lineWidth, cellStyles.fillColor);
        if (fillStyle) {
            doc.rect(cell.x, cursor.y, cell.width, cell.height, fillStyle);
        }
    }
    else if (typeof cellStyles.lineWidth === 'object') {
        doc.rect(cell.x, cursor.y, cell.width, cell.height, 'F');
        var sides = Object.keys(cellStyles.lineWidth);
        var lineWidth_1 = cellStyles.lineWidth;
        sides.map(function (side) {
            var fillStyle = common_1.getFillStyle(lineWidth_1[side], cellStyles.fillColor);
            drawBorderForSide(doc, cell, cursor, side, fillStyle || 'S', lineWidth_1[side]);
        });
    }
}
function drawBorderForSide(doc, cell, cursor, side, fillStyle, lineWidth) {
    var x1, y1, x2, y2;
    switch (side) {
        case 'top':
            x1 = cursor.x;
            y1 = cursor.y;
            x2 = cursor.x + cell.width;
            y2 = cursor.y;
            break;
        case 'left':
            x1 = cursor.x;
            y1 = cursor.y;
            x2 = cursor.x;
            y2 = cursor.y + cell.height;
            break;
        case 'right':
            x1 = cursor.x + cell.width;
            y1 = cursor.y;
            x2 = cursor.x + cell.width;
            y2 = cursor.y + cell.height;
            break;
        default:
            // default it will print bottom
            x1 = cursor.x;
            y1 = cursor.y + cell.height - lineWidth;
            x2 = cursor.x + cell.width;
            y2 = cursor.y + cell.height - lineWidth;
            break;
    }
    doc.getDocument().setLineWidth(lineWidth);
    doc.getDocument().line(x1, y1, x2, y2, fillStyle);
}
function getRemainingPageSpace(doc, table, isLastRow, cursor) {
    var bottomContentHeight = table.settings.margin.bottom;
    var showFoot = table.settings.showFoot;
    if (showFoot === 'everyPage' || (showFoot === 'lastPage' && isLastRow)) {
        bottomContentHeight += table.getFootHeight(table.columns);
    }
    return doc.pageSize().height - cursor.y - bottomContentHeight;
}
function addPage(doc, table, startPos, cursor, columns) {
    if (columns === void 0) { columns = []; }
    doc.applyStyles(doc.userStyles);
    if (table.settings.showFoot === 'everyPage') {
        table.foot.forEach(function (row) { return printRow(doc, table, row, cursor, columns); });
    }
    // Add user content just before adding new page ensure it will
    // be drawn above other things on the page
    table.callEndPageHooks(doc, cursor);
    var margin = table.settings.margin;
    common_1.addTableBorder(doc, table, startPos, cursor);
    nextPage(doc);
    table.pageNumber++;
    table.pageCount++;
    cursor.x = margin.left;
    cursor.y = margin.top;
    startPos.y = margin.top;
    if (table.settings.showHead === 'everyPage') {
        table.head.forEach(function (row) { return printRow(doc, table, row, cursor, columns); });
        doc.applyStyles(doc.userStyles);
    }
}
exports.addPage = addPage;
function nextPage(doc) {
    var current = doc.pageNumber();
    doc.setPage(current + 1);
    var newCurrent = doc.pageNumber();
    if (newCurrent === current) {
        doc.addPage();
    }
}
