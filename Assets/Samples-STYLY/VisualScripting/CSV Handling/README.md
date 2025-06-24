This sample demonstrates how to handle CSV files in VisualScripting.

The VisualScripting graph `Handle CSV` parses and generates GameObjects according to the CSV content.
The CSV file `Colors.csv` has a label and color values in each row.

```
label,R,G,B <--- This line is not included in the file.
red,255,0,0
green,0,255,0
blue,0,0,255
...
```

For each label, a new GameObject (Text) is created with the color assigned to it.

# Limitations
The VisualScripting graph `Handle CSV`
- can NOT parse Tab separated files. This is compatible only for comma separated CSVs.
- can NOT parse multiline strings.
- can NOT parse strings with comma in them.