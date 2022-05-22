using MLLab1;

string importFileName = "data.csv";
string exportFileName = "data.html";

DataManager dataManager = new DataManager();

dataManager.ImportData(importFileName);
dataManager.Calculate(Method.Nearest);
dataManager.ExportData(exportFileName);