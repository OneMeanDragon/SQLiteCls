# SQLiteCls

```vb
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim theDB As New SQLiteCls("test.DB3", 3)

        Dim AtTable As String = "ipbans"
        Dim Columns() As String = {"ip", "setby"}
        Dim Values() As String = {"'127.0.0.1'", "'server'"}
        theDB.InsertData(AtTable, Columns.Count, Columns, Values)

        Dim AtTable2 As String = "ipbans"
        Dim Columns2() As String = {"ip", "setby"}
        Dim Values2() As String = {"'10.1.1.1'", "'server'"}
        theDB.InsertData(AtTable2, Columns2.Count, Columns2, Values2)

        Dim test As Boolean = theDB.ItemExists("ipbans", "ip", "127.0.0.1")

        'theDB.DeleteItem("ipbans", "ip", "127.0.0.1") 'Delete the row
        theDB.UpdateData("ipbans", "ip", "127.0.0.1", "setby", "bob marley") 'update a section in the row
        theDB.UpdateData("ipbans", "ip", "127.0.0.1", "reason", "eat a dick")
        theDB.UpdateData("ipbans", "ip", "127.0.0.1", "ip", "255.255.255.255") 

        Dim dummystring As String = theDB.RetrieveData("ipbans", "setby", "ip", "255.255.255.255") 'get the value in 'setby'
    End Sub
```
