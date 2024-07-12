'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
var applyPlugin_1 = require("./applyPlugin");
var inputParser_1 = require("./inputParser");
var tableDrawer_1 = require("./tableDrawer");
var tableCalculator_1 = require("./tableCalculator");
var models_1 = require("./models");
exports.Table = models_1.Table;
var HookData_1 = require("./HookData");
exports.CellHookData = HookData_1.CellHookData;
var models_2 = require("./models");
exports.Cell = models_2.Cell;
exports.Column = models_2.Column;
exports.Row = models_2.Row;
// export { applyPlugin } didn't export applyPlugin
// to index.d.ts for some reason
function applyPlugin(jsPDF) {
    applyPlugin_1.default(jsPDF);
}
exports.applyPlugin = applyPlugin;
function autoTable(d, options) {
    var input = inputParser_1.parseInput(d, options);
    var table = tableCalculator_1.createTable(d, input);
    tableDrawer_1.drawTable(d, table);
}
// Experimental export
function __createTable(d, options) {
    var input = inputParser_1.parseInput(d, options);
    return tableCalculator_1.createTable(d, input);
}
exports.__createTable = __createTable;
function __drawTable(d, table) {
    tableDrawer_1.drawTable(d, table);
}
exports.__drawTable = __drawTable;
try {
    // eslint-disable-next-line @typescript-eslint/no-var-requires
    var jsPDF = require('jspdf');
    // Webpack imported jspdf instead of jsPDF for some reason
    // while it seemed to work everywhere else.
    if (jsPDF.jsPDF)
        jsPDF = jsPDF.jsPDF;
    applyPlugin(jsPDF);
}
catch (error) {
    // Importing jspdf in nodejs environments does not work as of jspdf
    // 1.5.3 so we need to silence potential errors to support using for example
    // the nodejs jspdf dist files with the exported applyPlugin
}
exports.default = autoTable;
