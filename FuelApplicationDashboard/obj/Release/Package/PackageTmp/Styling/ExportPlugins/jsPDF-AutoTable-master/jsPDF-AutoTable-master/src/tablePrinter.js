"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var common_1 = require("./common");
var getPageAvailableWidth = function (doc, table) {
    var margins = common_1.parseSpacing(table.settings.margin, 0);
    var availablePageWidth = doc.pageSize().width - (margins.left + margins.right);
    return availablePageWidth;
};
// get columns can be fit into page
var getColumnsCanFitInPage = function (doc, table, config) {
    if (config === void 0) { config = {}; }
    var _a;
    // get page width
    var availablePageWidth = getPageAvailableWidth(doc, table);
    var remainingWidth = availablePageWidth;
    // get column data key to repeat
    var horizontalPageBreakRepeat = table.settings.horizontalPageBreakRepeat;
    var repeatColumn = null;
    var cols = [];
    var columns = [];
    var len = table.columns.length;
    var i = config && config.start ? config.start : 0;
    // code to repeat the given column in split pages
    if (horizontalPageBreakRepeat != null) {
        repeatColumn = table.columns.find(function (item) {
            return item.dataKey === horizontalPageBreakRepeat ||
                item.index === horizontalPageBreakRepeat;
        });
        if (repeatColumn) {
            cols.push(repeatColumn.index);
            columns.push(table.columns[repeatColumn.index]);
            remainingWidth = remainingWidth - repeatColumn.wrappedWidth;
        }
    }
    while (i < len) {
        if (((_a = repeatColumn) === null || _a === void 0 ? void 0 : _a.index) === i) {
            i++; // prevent columnDataKeyToRepeat to be pushed twice in a page
            continue;
        }
        var colWidth = table.columns[i].wrappedWidth;
        if (remainingWidth < colWidth) {
            // check if it's first column in the sequence then add it into result
            if (i === 0 || i === config.start) {
                // this cell width is more than page width set it available pagewidth
                /* table.columns[i].wrappedWidth = availablePageWidth
                table.columns[i].minWidth = availablePageWidth */
                cols.push(i);
                columns.push(table.columns[i]);
            }
            // can't print more columns in same page
            break;
        }
        cols.push(i);
        columns.push(table.columns[i]);
        remainingWidth = remainingWidth - colWidth;
        i++;
    }
    return { colIndexes: cols, columns: columns, lastIndex: i };
};
var calculateAllColumnsCanFitInPage = function (doc, table) {
    // const margins = table.settings.margin;
    // const availablePageWidth = doc.pageSize().width - (margins.left + margins.right);
    var allResults = [];
    var index = 0;
    var len = table.columns.length;
    while (index < len) {
        var result = getColumnsCanFitInPage(doc, table, {
            start: index === 0 ? 0 : index,
        });
        if (result && result.columns && result.columns.length) {
            index = result.lastIndex;
            allResults.push(result);
        }
        else {
            index++;
        }
    }
    return allResults;
};
exports.default = {
    getColumnsCanFitInPage: getColumnsCanFitInPage,
    calculateAllColumnsCanFitInPage: calculateAllColumnsCanFitInPage,
    getPageAvailableWidth: getPageAvailableWidth,
};
