Imports System.Data.SQLite 'NuGet package = [System.Data.SQLite.Core]

Public Class SQLiteCls
    Private LockObject As Object
    ' File
    Private Property DBFile() As String
    ' Format string
    Private ConnectionString As String
    ' Connection String
    Private DBCon As SQLiteConnection

    Private ReadOnly Property DataBaseExists() As Boolean
        Get
            Return System.IO.File.Exists(DBFile)
        End Get
    End Property

    Public Sub New(ByVal FileName As String, ByVal VersionID As Integer)
        Try
            LockObject = New Object
            DBFile = FileName

            ConnectionString = String.Format("Data Source={0};Version={1}", DBFile, VersionID.ToString())
            DBCon = New SQLiteConnection(ConnectionString)

            If DataBaseExists Then
                DBCon.Open()
            Else
                DBCon.Open() 'Now it does
                ' Create the entire initial database file.
                CreateDBTables()
            End If

        Catch ex As Exception
            MessageBox.Show("Error: (SQLite New()) " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' DataValue is raw data if your inserting a string remember to incase it 'value'
    ''' </summary>
    ''' <param name="TableID">the table in the database</param>
    ''' <param name="ValueCount">number of values you are inserting</param>
    ''' <param name="ItemName">the names of the columns you are adding the data to</param>
    ''' <param name="DataValue">the values to insert in the order of the columns</param>
    Public Sub InsertData(ByVal TableID As String, ByVal ValueCount As Integer, ByVal ItemName() As String, ByVal DataValue() As String)
        If ValueCount = 0 Then Return 'no values in the buffers
        If ItemName.Count > ValueCount OrElse ItemName.Count < ValueCount Then Return 'not enough columns
        If DataValue.Count > ValueCount OrElse DataValue.Count < ValueCount Then Return 'not enough values

        Dim InsertCommandValue As String = "INSERT INTO " & TableID
        Dim ColumnsData As String = "("
        Dim ValuesData As String = "VALUES("

        For i As Integer = 0 To ValueCount - 1
            If i < 1 Then
                ColumnsData &= ItemName(i)
                ValuesData &= DataValue(i)
            Else
                ColumnsData &= "," & ItemName(i)
                ValuesData &= "," & DataValue(i)
            End If
        Next
        ColumnsData &= ")"
        ValuesData &= ")"

        InsertCommandValue &= ColumnsData & " " & ValuesData & ";"

        'Lock the querry
        SyncLock LockObject
            Dim InsertCommand As New SQLiteCommand(InsertCommandValue, DBCon)
            InsertCommand.ExecuteNonQuery()
        End SyncLock

    End Sub

    ''' <summary>
    ''' DataValue is raw data if your inserting a string remember to incase it 'value'
    ''' </summary>
    ''' <param name="TableID">the table in the database</param>
    ''' <param name="ItemName">the name of the column you are adding the data to</param>
    ''' <param name="DataValue">the value to insert in the order of the columns</param>
    Public Sub InsertData(ByVal TableID As String, ByVal ItemName As String, ByVal DataValue As String)
        If ItemExists(TableID, ItemName, DataValue) Then Return 'This Item already exists in the database
        Dim InsertCommandValue As String = "INSERT INTO " & TableID
        Dim ColumnsData As String = "(" & ItemName & ")"
        Dim ValuesData As String = "VALUES(" & DataValue & ")"
        InsertCommandValue &= ColumnsData & " " & ValuesData & ";"

        SyncLock LockObject
            Dim InsertCommand As New SQLiteCommand(InsertCommandValue, DBCon)
            InsertCommand.ExecuteNonQuery()
        End SyncLock
    End Sub

    Public Sub UpdateData(ByVal TableID As String, ByVal SearchColumnID As String, ByVal SearchID As String, ByVal ColumnID As String, ByVal DataValue As String)

        SyncLock LockObject
            Dim UpdateCommand As New SQLiteCommand("UPDATE `" & TableID & "` SET `" & ColumnID & "` =@VALUE WHERE `" & SearchColumnID & "` =@SEARCHVALUE", DBCon)
            UpdateCommand.Parameters.AddWithValue("@VALUE", DataValue)
            UpdateCommand.Parameters.AddWithValue("@SEARCHVALUE", SearchID)
            UpdateCommand.ExecuteNonQuery()
        End SyncLock

    End Sub

    Public Function RetrieveData(ByVal TableID As String, ByVal GetColumnID As String, ByVal WhereColumnID As String, ByVal ItemName As String) As String
        Dim outStr As String = ""

        SyncLock LockObject
            Dim ReadDataCommand As New SQLiteCommand("SELECT " & GetColumnID & " FROM " & TableID & " WHERE " & WhereColumnID & "=@VALUE", DBCon)
            ReadDataCommand.Parameters.AddWithValue("@VALUE", ItemName)
            Dim CommandReader As SQLiteDataReader = ReadDataCommand.ExecuteReader()
            CommandReader.Read() 'Should check if this row is even in the database first really
            outStr = CommandReader.GetValue(0)
        End SyncLock

        Return outStr
    End Function

    Public Function ItemExists(ByVal TableID As String, ByVal ColumnID As String, ByVal ItemName As String) As Boolean
        Dim SelectCommand As New SQLiteCommand("SELECT * FROM `" & TableID & "` WHERE `" & ColumnID & "` =@ITEMNAME", DBCon)
        SelectCommand.Parameters.AddWithValue("@ITEMNAME", ItemName)
        Dim Exists As Boolean

        SyncLock LockObject
            Dim SelectReader As SQLiteDataReader = SelectCommand.ExecuteReader()
            Exists = SelectReader.HasRows
        End SyncLock

        Return Exists
    End Function

    Public Sub DeleteItem(ByVal TableID As String, ByVal ColumnID As String, ByVal ItemName As String)

        SyncLock LockObject
            Dim DeleteCommand As New SQLiteCommand("DELETE FROM `" & TableID & "` WHERE `" & ColumnID & "` =@ITEMNAME", DBCon)
            DeleteCommand.Parameters.AddWithValue("@ITEMNAME", ItemName)
            DeleteCommand.ExecuteNonQuery()
        End SyncLock

    End Sub

    Private Sub CreateDBTables()
        ' IP BAN TABLE [create ur own tables blah blah]
        Dim IPBanTable As String = "CREATE TABLE `ipbans` (" &
                                        "`ip` TEXT," &
                                        "`setby` TEXT," &
                                        "`reason` TEXT," &
                                        "`timestarted` INTEGER DEFAULT (strftime('%s','now'))," &
                                        "`amountoftime` INTEGER DEFAULT (0)" &
                                   ");"
        Dim cmdIPBan As New SQLiteCommand(IPBanTable, DBCon)
        cmdIPBan.ExecuteNonQuery()
    End Sub
End Class
