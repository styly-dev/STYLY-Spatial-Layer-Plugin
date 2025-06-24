You will learn the concept of making POST requests in Unity.
The name and score will be sent as JSON.
Please check the graph in this folder.

The posted score will be recorded in a Google Spreadsheet.

Here is a sample for receiving POST requests with Google Apps Script.
```google-apps-script
function doPost(e) {
  const sheet = SpreadsheetApp.getActiveSpreadsheet().getActiveSheet();
  const requestData = JSON.parse(e.postData.contents);

  // Insert new data on the second line
  sheet.insertRowBefore(2); // Insert a new line on the second line
  sheet.getRange(2, 1, 1, 2).setValues([[requestData.name, requestData.score]]);

  return ContentService.createTextOutput(JSON.stringify({
    status: "success",
    message: "Score recorded to the second row of the sheet",
  })).setMimeType(ContentService.MimeType.JSON);
}
```
