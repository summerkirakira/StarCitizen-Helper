﻿Imports System.ComponentModel
Imports System.IO
Imports System.Net

Public Class MainForm

    '<----------------------------------- Form
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MAIN_THREAD.WL_Mod.Text_Label_Bottom = "Внимание! Все действия во вкладке [" & Me.TabPage_Patch.Text & "] Вы выполняете на свой страх и риск." & vbNewLine & "Задействование функции [" & Me.WL_Mod.Text_Button_Enable & "] нарушит условия лицензионного соглашения с CIG." & vbNewLine & vbNewLine & "Для включения или выключения модификации необходимо загрузить ядро модификаций и локализацию входящие в пакет обновлений. Для этого перейдите во вкладку [" & Me.TabPage_Packages.Text & "] и следуйте инструкциям." & vbNewLine & vbNewLine & "Примечание: Если был загружен и установлен новый пакет обновлений, то необходимо выключить и включить ядро модификаций, это задействует соответствующую версию ядра для соответствующей локализации. Программа не вносит изменния в исполняемый фалы игры, но модифицирует память и когда игра запускается, это значительно сложнее выявить." & vbNewLine & vbNewLine & "Автор программы против читов и бесчестной игры, данная программа не несет подобного функционала."
        MAIN_THREAD.WL_Pack.Text_Label_Bottom = "Для загрузки пакета обновлений выберите в выпадающем списке актуальный пакет обновлений и нажмите [" & Me.WL_Pack.Text_Button_Download & "]. По завершении загрузки нажмите [" & Me.WL_Pack.Text_Button_InstallFull & "] - это установит необходимые файлы локализации в папку игры." & vbNewLine & "Для активации локализации требуется установить и включить ядро модификаций, для этого перейдите во вкладку [" & Me.TabPage_Patch.Text & "] и нажмите [" & Me.WL_Mod.Text_Button_Enable & "]." & vbNewLine & vbNewLine & "Примечание: Пакет локализации [Master] - это последняя версия сборки, еще не прошедшей проверку (не рекомендуется к установке)."

        MAIN_THREAD.WL_SysUpdate.Label_AutoUpdate.Text = "AAA"
    End Sub


    'Private Sub CheckBox_FSOWatcher_CheckedChanged(sender As Object, e As EventArgs)
    'On Error Resume Next
    '_VARS.FileWatcher = Me.CheckBox_Watcher.Checked
    ' _WATCHFILE_THREAD.PushWatchFiles = True
    '_INI._Write("CONFIGURATION", "FILES_WATCHER", BoolToString(_VARS.FileWatcher))
    ' End Sub

    Private Sub MainForm_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Me.Visible = True Then
            Me.Timer_LOG.Enabled = True
        Else
            Me.Timer_LOG.Enabled = False
        End If
    End Sub
    '-----------------------------------> 'Form

    '<----------------------------------- Download and update


    '-----------------------------------> 'Download and update

    '<----------------------------------- Process killer
    Private Sub CheckBox_KillerThread_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_KillerThread.CheckedChanged
        If Me.CheckBox_KillerThread.Checked = True Then
            If _KEYS.ThreadState = False Then _KEYS.ThreadStart()
            Me.Label_KillerThread.Text = "Функция активирована"
        Else
            _KEYS.ThreadStop()
            Me.Label_KillerThread.Text = "Функция выключена"
        End If

        _VARS.PKillerEnabled = Me.CheckBox_KillerThread.Checked
        KillerThread_ToolStripMenuItem.Checked = _VARS.PKillerEnabled
        _INI._Write("CONFIGURATION", "PKILLER_ENABLED", BoolToString(_VARS.PKillerEnabled))
    End Sub

    Private Sub SetKeyKill_Button_Click(sender As Object, e As EventArgs) Handles SetKeyKill_Button.Click
        SetKeyKill_Button.Enabled = False
        If _KEYS.ThreadState = False Then _KEYS.ThreadStart()
        Dim NewKey As Class_KEYS.Class_KEY = _KEYS._SetNewKey
        _VARS.PKillerKeyID = NewKey.ID
        _KEYS._Clear()
        _KEYS._Add(_VARS.PKillerKeyID, KeyModifierListToKeys(Me.ProcessKillerModKey_ComboBox.SelectedIndex), "KillProcess", _VARS.PKillerKeyID)

        _INI._Write("CONFIGURATION", "PKILLER_KEY", _VARS.PKillerKeyID)
        My.Computer.Audio.Play(My.Resources.process_kill, AudioPlayMode.Background)
        If _VARS.PKillerEnabled = False Then _KEYS.ThreadStop()
        Me.SetKeyKill_Button.Enabled = True
        UpdateInterface()
    End Sub

    Private Sub ProcessKillerModKey_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ProcessKillerModKey_ComboBox.SelectedIndexChanged
        If Initialization = True Then Exit Sub
        _VARS.PKillerKeyMod = Me.ProcessKillerModKey_ComboBox.SelectedIndex
        Me.Label_ProcessKillerModKey.Text = "Имя клавиши: " & Chr(34) & _KEYS._GetKeyNameByID(KeyModifierListToKeys(Me.ProcessKillerModKey_ComboBox.SelectedIndex)) & Chr(34) & vbNewLine & "ID клавиши: " & KeyModifierListToKeys(Me.ProcessKillerModKey_ComboBox.SelectedIndex)
        _KEYS._Clear()
        _KEYS._Add(_VARS.PKillerKeyID, KeyModifierListToKeys(Me.ProcessKillerModKey_ComboBox.SelectedIndex), "KillProcess", _VARS.PKillerKeyID)
        _INI._Write("CONFIGURATION", "PKILLER_MOD", _VARS.PKillerKeyMod)
    End Sub

    Private Sub AddProccessKill_Button_Click(sender As Object, e As EventArgs) Handles AddProccessKill_Button.Click
        If Len(Trim(Me.AddProccessKill_TextBox.Text)) = 0 Then
            _LOG._sAdd("WINDOW_FORM", "Требуется указать имя процесса")
            Exit Sub
        End If
        For Each line In Me.ProccessKill_CheckedListBox.Items
            If LCase(line) = LCase(AddProccessKill_TextBox.Text) Then
                _LOG._sAdd("WINDOW_FORM", "Указанный процесс уже в списке")
                Exit Sub
            End If
        Next
        Me.ProccessKill_CheckedListBox.Items.Add(AddProccessKill_TextBox.Text)
        Me.AddProccessKill_TextBox.Text = Nothing
        Me.RemoveProccessKill_Button.Enabled = True
        ProccessKill_CheckedListBox_Update(ProccessKill_CheckedListBox, False)
    End Sub

    Private Sub RemoveProccessKill_Button_Click(sender As Object, e As EventArgs) Handles RemoveProccessKill_Button.Click
        If Me.ProccessKill_CheckedListBox.SelectedIndex = -1 Then
            _LOG._sAdd("WINDOW_FORM", "Требуется выбрать имя процесса")
            Exit Sub
        End If
        Me.ProccessKill_CheckedListBox.Items.RemoveAt(ProccessKill_CheckedListBox.SelectedIndex)
        If ProccessKill_CheckedListBox.Items.Count = 0 Then RemoveProccessKill_Button.Enabled = False
        ProccessKill_CheckedListBox_Update(ProccessKill_CheckedListBox, False)
    End Sub

    Private Sub AddProccessKill_TextBox_TextChanged(sender As Object, e As EventArgs) Handles AddProccessKill_TextBox.TextChanged
        Me.ProccessList_ListBox.Items.Clear()
        If Len(AddProccessKill_TextBox.Text) < 1 Then Exit Sub
        Dim pList As List(Of Process) = _PROCESS._Get(AddProccessKill_TextBox.Text)
        For Each elem In pList
            Me.ProccessList_ListBox.Items.Add(elem.ProcessName)
        Next
    End Sub

    Private Sub ProccessKill_CheckedListBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ProccessKill_CheckedListBox.SelectedIndexChanged
        ProccessKill_CheckedListBox_Update(ProccessKill_CheckedListBox, False)
    End Sub

    Private Sub ProccessList_ListBox_MouseClick(sender As Object, e As MouseEventArgs) Handles ProccessList_ListBox.MouseClick
        On Error Resume Next
        If Len(ProccessList_ListBox.SelectedItem.ToString) > 0 Then
            Me.AddProccessKill_TextBox.Text = ProccessList_ListBox.SelectedItem.ToString
        End If
    End Sub
    '-----------------------------------> 'Process killer

    '<----------------------------------- Profiles
    Private Sub Button_ToLIVE_Click(sender As Object, e As EventArgs) Handles Button_ToLIVE.Click
        If _VARS.GameProcessKillerEnabled = True Then
            If _VARS.GameProcessMain IsNot Nothing Then _PROCESS._Kill(_VARS.GameProcessMain, False, True)
            If _VARS.GameProcessLauncher IsNot Nothing Then _PROCESS._Kill(_VARS.GameProcessLauncher, False, True)
        End If
        RenameLIVEFolder("LIVE", False)
    End Sub

    Private Sub Button_ToPTU_Click(sender As Object, e As EventArgs) Handles Button_ToPTU.Click
        If _VARS.GameProcessKillerEnabled = True Then
            If _VARS.GameProcessMain IsNot Nothing Then _PROCESS._Kill(_VARS.GameProcessMain, False, True)
            If _VARS.GameProcessLauncher IsNot Nothing Then _PROCESS._Kill(_VARS.GameProcessLauncher, False, True)
        End If
        RenameLIVEFolder("PTU", False)
    End Sub

    Private Sub Button_ToEPTU_Click(sender As Object, e As EventArgs) Handles Button_ToEPTU.Click
        If _VARS.GameProcessKillerEnabled = True Then
            If _VARS.GameProcessMain IsNot Nothing Then _PROCESS._Kill(_VARS.GameProcessMain, False, True)
            If _VARS.GameProcessLauncher IsNot Nothing Then _PROCESS._Kill(_VARS.GameProcessLauncher, False, True)
        End If
        RenameLIVEFolder("EPTU", False)
    End Sub

    Private Sub CheckBox_BeforeKillProcess_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_BeforeKillProcess.CheckedChanged
        _VARS.GameProcessKillerEnabled = Me.CheckBox_BeforeKillProcess.Checked
        BeforeKillProcess_ToolStripMenuItem.Checked = _VARS.PKillerEnabled
        _INI._Write("EXTERNAL", "PROFILES_PROCESS_KILL_ENABLED", BoolToString(_VARS.GameProcessKillerEnabled))
    End Sub
    '-----------------------------------> 'Profiles

    '<----------------------------------- Log
    Private Sub ClearLog_Button_Click(sender As Object, e As EventArgs) Handles ClearLog_Button.Click
        Me.TextBox_Debug.Text = Nothing
    End Sub
    '-----------------------------------> 'Log

    '<----------------------------------- Menu
    Private Sub ModOn_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModOn_ToolStripMenuItem.Click
        'Mod ON menu
        Me.WL_Mod.Button_Enable_Click(sender, e)
    End Sub

    Private Sub ModOff_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModOff_ToolStripMenuItem.Click
        'Mod OFF menu
        Me.WL_Mod.Button_Disable_Click(sender, e)
    End Sub

    Private Sub InstallAll_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InstallAll_ToolStripMenuItem.Click
        'Full install menu
        Me.WL_Pack.Button_InstallFull_Click(sender, e)
    End Sub

    Private Sub KillerThread_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KillerThread_ToolStripMenuItem.Click
        'Process killer On/Off menu
        If _VARS.PKillerEnabled = True Then
            _VARS.PKillerEnabled = False
        Else
            _VARS.PKillerEnabled = True
        End If
        Me.CheckBox_KillerThread.Checked = _VARS.PKillerEnabled
        Me.KillerThread_ToolStripMenuItem.Checked = _VARS.PKillerEnabled
    End Sub

    Private Sub KillProcesses_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KillProcesses_ToolStripMenuItem.Click
        'Process killer force kill menu
        _KEYS._ForceClick(_VARS.PKillerKeyID)
    End Sub

    Private Sub BeforeKillProcess_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BeforeKillProcess_ToolStripMenuItem.Click
        'Profiles kill processes
        If _VARS.GameProcessKillerEnabled = True Then
            _VARS.GameProcessKillerEnabled = False
        Else
            _VARS.GameProcessKillerEnabled = True
        End If
        Me.CheckBox_BeforeKillProcess.Checked = _VARS.GameProcessKillerEnabled
        Me.BeforeKillProcess_ToolStripMenuItem.Checked = _VARS.GameProcessKillerEnabled
    End Sub

    Private Sub ToLIVE_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToLIVE_ToolStripMenuItem.Click
        'Profiles to LIVE menu
        Button_ToLIVE_Click(sender, e)
    End Sub

    Private Sub ToPTU_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToPTU_ToolStripMenuItem.Click
        'Profiles to PTU menu
        Button_ToPTU_Click(sender, e)
    End Sub

    Private Sub ToEPTU_ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToEPTU_ToolStripMenuItem.Click
        'Profiles to EPTU menu
        Button_ToEPTU_Click(sender, e)
    End Sub

    Private Sub ShowWinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowWinToolStripMenuItem.Click
        'Hide menu
        If Me.Visible = False Then
            Me.Show()
            Me.ShowWinToolStripMenuItem.Text = "Скрыть программу"
        Else
            Me.Hide()
            Me.ShowWinToolStripMenuItem.Text = "Отобразить программу"
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        'Exit menu
        Application.Exit()
    End Sub
    '-----------------------------------> 'Menu

    '<----------------------------------- Other form elements
    Private Sub Timer_LOG_Tick(sender As Object, e As EventArgs) Handles Timer_LOG.Tick
        On Error Resume Next
        If _LOG.Buffer IsNot Nothing Then
            Me.TextBox_Debug.AppendText(_LOG.Buffer)
            _LOG.Buffer = Nothing
            Me.TextBox_Debug.SelectionStart = Me.TextBox_Debug.TextLength - 1
            Me.TextBox_Debug.ScrollToCaret()
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        ShowWinToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl.SelectedIndexChanged
        On Error Resume Next
        If TabControl.SelectedTab Is TabPage_Debug Then
            Me.TextBox_Debug.SelectionStart = Me.TextBox_Debug.TextLength - 1
            Me.TextBox_Debug.ScrollToCaret()
        End If
    End Sub
    '-----------------------------------> 'Other form elements

    '<----------------------------------- Form logic
    Public Sub UpdateInterface()
        RenameLIVEFolder(Nothing)

        'Me.CheckBox_FSOWatcher.Checked = _VARS.FileWatcher

        'Menu
        ModOn_ToolStripMenuItem.Enabled = False
        ModOff_ToolStripMenuItem.Enabled = False
        If Me.WL_Mod.Property_GameExeFilePath IsNot Nothing Then
            If Me.WL_Mod.Property_ModStatus = True Then
                ModOff_ToolStripMenuItem.Enabled = True
            Else
                ModOn_ToolStripMenuItem.Enabled = True
            End If
        End If

        If WL_Pack.Property_Path_File_Download IsNot Nothing Then
            InstallAll_ToolStripMenuItem.Enabled = True
        Else
            InstallAll_ToolStripMenuItem.Enabled = False
        End If

        'PKIller
        Me.CheckBox_KillerThread.Checked = _VARS.PKillerEnabled
        Me.KillerThread_ToolStripMenuItem.Checked = _VARS.PKillerEnabled
        Me.ProcessKillerModKey_ComboBox.SelectedIndex = _VARS.PKillerKeyMod
        If _VARS.PKillerKeyID = 0 Then
            Me.Label_SetKeyKill.Text = "Клавиша модификатор не задана"
        Else
            Me.Label_ProcessKillerModKey.Text = "Имя клавиши: " & Chr(34) & _KEYS._GetKeyNameByID(KeyModifierListToKeys(Me.ProcessKillerModKey_ComboBox.SelectedIndex)) & Chr(34) & vbNewLine & "ID клавиши: " & KeyModifierListToKeys(Me.ProcessKillerModKey_ComboBox.SelectedIndex)
        End If

        If ProccessKill_CheckedListBox_Update(Me.ProccessKill_CheckedListBox, True) > 0 Then Me.RemoveProccessKill_Button.Enabled = True
        If _VARS.PKillerKeyID = 0 Then
            Me.Label_SetKeyKill.Text = "Горячая клавиша не задана"
        Else
            Me.Label_SetKeyKill.Text = "Имя клавиши: " & Chr(34) & _KEYS._GetKeyNameByID(_VARS.PKillerKeyID) & Chr(34) & vbNewLine & "ID клавиши: " & _VARS.PKillerKeyID
            _KEYS._Add(_VARS.PKillerKeyID, KeyModifierListToKeys(ProcessKillerModKey_ComboBox.SelectedIndex), "KillProcess", _VARS.PKillerKeyID)
        End If

        'Profiles
        Me.CheckBox_BeforeKillProcess.Checked = _VARS.GameProcessKillerEnabled

    End Sub
    '-----------------------------------> 'Form logic

    '<----------------------------------- 'Callback
    Sub GamePathUpdate(Path As String) Handles WL_Mod._Event_GameExeFile_Update_After
        If Path Is Nothing Then Exit Sub

        MAIN_THREAD.WL_Mod._Update()
        MAIN_THREAD.WL_Mod.Property_PatchDstFilePath = _FSO._CombinePath(MAIN_THREAD.WL_Mod.Property_GameExeFolderPath, MAIN_THREAD.WL_Mod.Property_PatchDstFileName)

        MAIN_THREAD.WL_Pack.Property_Path_Folder_Meta = MAIN_THREAD.WL_Mod.Property_GameRootFolderPath
        MAIN_THREAD.WL_Pack.Property_Path_File_Meta = _FSO._CombinePath(MAIN_THREAD.WL_Pack.Property_Path_Folder_Meta, MAIN_THREAD.WL_Pack.Property_Name_File_Meta)
    End Sub

    Sub Upd_Controls_Enabled(Enabled As Boolean) Handles WL_Pack._Event_Controls_Enabled_Before, WL_Pack._Event_Controls_Enabled_After
        MAIN_THREAD.WL_Mod.Enabled = Enabled
    End Sub

    Sub Mod_Controls_Enabled(Enabled As Boolean) Handles WL_Mod._Event_Controls_Enabled_Before, WL_Mod._Event_Controls_Enabled_After
        MAIN_THREAD.WL_Pack.Enabled = Enabled
    End Sub

    Sub DownloadAfter(DownloadFrom As String, DownloadTo As String, e As WL_Download.DownloadProgressElement) Handles WL_Pack._Event_Download_After
        Dim result As New ResultClass(Me)
        result.ValueString = DownloadTo
        _FSO._DeleteFile(_FSO._CombinePath(MAIN_THREAD.WL_Pack.Property_Path_Folder_Download, MAIN_THREAD.WL_Mod.Property_PatchSrcFileName))
        If _FSO.ZIP.UnzipFileToFolder(MAIN_THREAD.WL_Pack.Property_Path_File_Download, "." & MAIN_THREAD.WL_Mod.Property_PatchSrcFileName, _FSO._CombinePath(MAIN_THREAD.WL_Pack.Property_Path_Folder_Download, MAIN_THREAD.WL_Mod.Property_PatchSrcFileName)) = False Then result.Err._Flag = True : result.Err._Description_App = "Не удалось извлечь ядро из загруженного пакета локадизации"
        MAIN_THREAD.WL_Mod.Property_PatchSrcFilePath = _FSO._CombinePath(MAIN_THREAD.WL_Pack.Property_Path_Folder_Download, MAIN_THREAD.WL_Mod.Property_PatchSrcFileName)
        MAIN_THREAD.WL_Mod.Property_ModInPackFileVersion = MAIN_THREAD.WL_Pack.Property_PackInPackVersion
        MAIN_THREAD.WL_Mod._Update()
        Me.UpdateInterface()
    End Sub

    Sub ModStatus_Click() Handles WL_Mod._Event_PatchDisable_Click_After, WL_Mod._Event_PatchEnable_Click_After
        Me.UpdateInterface()
    End Sub

    Sub UpdInstallFull_Click() Handles WL_Pack._Event_InstallFull_Button_Click_After
        Me.UpdateInterface()
    End Sub



    '-----------------------------------> 'Callback
End Class