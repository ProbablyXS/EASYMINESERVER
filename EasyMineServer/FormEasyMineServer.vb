﻿Imports System.Drawing
Imports System.IO
Imports System.Net

Public Class FormEasyMineServer

    Public Locationappdataroaming1 As String = Path.GetTempPath.ToString
    Public WithEvents DownloadFile As New WebClient
    Dim cpu As New PerformanceCounter("Processor information", "% processor time", "_Total")
    Dim MEM As New PerformanceCounter("Memory", "% Committed Bytes In Use")
    Public Shared p As New Process()
    Dim code As String
    Dim k As String
    Public LocationAppdata As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
    Public Shared vServerMinecraft As String
    Public Shared INIFILE As New clsIni(Environment.CurrentDirectory & "\config.ini")
    Public Conflink As New clsIni(LocationAppdata & "\EASYMINESERVER\CONFIG\ConfLink.ini")
    Public Shared maj As String = "1.9.6"

    Public Async Sub DownloadProgression(sender As Object, ByVal e As DownloadProgressChangedEventArgs) Handles DownloadFile.DownloadProgressChanged

        k = e.ProgressPercentage.ToString & "%"
        RichTextBox1.ForeColor = Color.White
        RichTextBox1.Text = (k & vbCr)
        Await Task.Delay(2500)

        Await (Task.Delay(2500))

        If (k = "100%") Then

            RichTextBox1.AppendText(vbCrLf & "Download Finished")
            startserver()
            k = "0%"

        End If

    End Sub

    <Obsolete>
    Private Sub FormEasyMineServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'JAVA VERIFICATION'
        Dim regKey As Microsoft.Win32.RegistryKey
        regKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\JavaSoft", True)
        If (regKey Is Nothing) Then

            MsgBox("Install JAVA before launching this software", MsgBoxStyle.Information)
            Process.Start("https://java.com/en/download/manual.jsp")
            Close()
        End If
        'JAVA VERIFICATION'

        clearcache()

        'MAJ'
        Try

            If (DownloadFile.DownloadString("https://raw.githubusercontent.com/XsplitS/EASYMINESERVER/master/VERSION/VERSION.conf") = maj) Then

            Else

                Try
                    Dim msgyesno As Integer = MsgBox("Update available, Do you want to launch it ?", MsgBoxStyle.YesNo + MsgBoxStyle.Information)

                    If (msgyesno = vbYes) Then


                        Process.Start("UPDATE.exe")
                        Close()

                    End If

                Catch ex As Exception

                    MsgBox("UPDATE.exe not found", MsgBoxStyle.Critical)

                End Try

            End If

        Catch ex As Exception

            MsgBox(ex.Message & " No connection ?, You switch in offline mode.", MsgBoxStyle.Information)

        End Try

        If (File.Exists(Locationappdataroaming1 & "\Download\UPDATE.exe")) Then

            File.Delete("config.ini")
            File.Copy(Locationappdataroaming1 & "\Download\UPDATE.exe", "UPDATE.exe", True)
            Directory.Delete(Locationappdataroaming1 & "\Download", True)

        Else



        End If


        'GET IPV4 IN RAW'
        Try

            Label5.Text = Dns.GetHostByName(Dns.GetHostName).AddressList(0).ToString
            Label4.Text = DownloadFile.DownloadString("https://api.ipify.org/")

        Catch ex As Exception

            Label3.Text = "Public ip: ??"

        End Try
        'GET IPV4 IN RAW'


        If (My.Computer.FileSystem.FileExists("config.ini")) Then

            Try

                checkconfigfile()


            Catch ex As Exception

                MsgBox("Le fichier 'CONFIG.INI' est endommagé !", MsgBoxStyle.Exclamation)

                File.Delete("config.ini")

                File.WriteAllText("config.ini", My.Resources.Config)

                checkconfigfile()

            End Try


        Else

            File.WriteAllText("config.ini", My.Resources.Config)
            checkconfigfile()

        End If

        Directory.CreateDirectory(LocationAppdata & "\EASYMINESERVER\CONFIG")

        Control.CheckForIllegalCrossThreadCalls = False

        Try
            'Download files'
            DownloadFile.DownloadFile("https://raw.githubusercontent.com/XsplitS/EASYMINESERVER/master/VERSION/ConfLink.ini", LocationAppdata & "\EASYMINESERVER\CONFIG\ConfLink.ini")
            DownloadFile.DownloadFile("https://raw.githubusercontent.com/XsplitS/EASYMINESERVER/master/VERSION/VersionSM.ini", LocationAppdata & "\EASYMINESERVER\CONFIG\VersionSM.ini")

        Catch ex As Exception

            MsgBox("Problem with your connection ?", MsgBoxStyle.Exclamation)

        End Try

        clearcache()

        sound_click()

        commandArgs()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        sound_click()
        DisableAll()

        launchServer()

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click

        sound_click()
        About.Show()
        Me.Hide()

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        Try

            BWSTARTSERVER.RunWorkerAsync()

        Catch ex As Exception

        End Try



    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        sound_click()

        Try

            p.StandardInput.WriteLine(TextBox1.Text)
            code = TextBox1.Text
            TextBox1.Clear()

        Catch ex As Exception

        End Try

    End Sub

    Private Async Function BWSTARTSERVER_DoWorkAsync(sender As Object, e As System.ComponentModel.DoWorkEventArgs) As Task Handles BWSTARTSERVER.DoWork

        If (code = "stop") Then

            Button2.Enabled = False
            TextBox1.ReadOnly = True
            TextBox1.Clear()

            Timer2.Stop()

            code = ""

            Button1.Enabled = True

        End If

        If (code = "/stop") Then

            Button2.Enabled = False
            TextBox1.ReadOnly = True
            TextBox1.Clear()

            Timer2.Stop()

            code = ""

            Button1.Enabled = True

        End If


        Try

            If (RichTextBox1.Lines.Count > 50) Then


                Dim richtxt As String = RichTextBox1.Text
                Dim pos As Integer = richtxt.IndexOf(vbLf)
                RichTextBox1.Text = richtxt.Substring(pos + 1, richtxt.Length - pos - 1)

            End If

        Catch ex As Exception

        End Try

        Try

            Dim ll As String = p.StandardOutput.ReadLine & vbCrLf
            RichTextBox1.AppendText(ll)

            RichTextBox1.ScrollToCaret()

        Catch ex As Exception

        End Try

    End Function

    Private Sub FormEasyMineServer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try


            p.StandardInput.WriteLine("stop")
            p.StandardInput.WriteLine("/stop")
            p.Kill()
            p.Close()

            Dim ServerClose As Process() = Process.GetProcessesByName("java")
            Array.ForEach(ServerClose, Sub(p As Process) p.Kill())

            Button2.Enabled = False
            TextBox1.ReadOnly = True
            TextBox1.Clear()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown

        If (e.KeyCode = Keys.Enter) Then

            Try

                p.StandardInput.WriteLine(TextBox1.Text)
                code = TextBox1.Text
                TextBox1.Clear()

            Catch ex As Exception

            End Try

        End If

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TimerCPU.Tick

        Try

            Dim cpuload As Integer = CDec(cpu.NextValue.ToString())
            Label1.Text = "Processor: " & cpuload & "%"

            If (cpuload >= 80) Then

                Label1.ForeColor = Color.Red

                If (INIFILE.GetString("CONFIG", "SOUNDALERT", "") = "TRUE") Then

                    My.Computer.Audio.Play(My.Resources.Alert, AudioPlayMode.Background)

                End If

            Else

                Label1.ForeColor = Color.White

            End If

        Catch ex As Exception



        End Try

    End Sub

    Private Sub TimerMEM_Tick(sender As Object, e As EventArgs) Handles TimerMEM.Tick

        Try

            Dim memload As Integer = CDec(MEM.NextValue.ToString())
            Label6.Text = "MEM: " & memload & "%"

            If (memload >= 80) Then

                Label1.ForeColor = Color.Red

                If (INIFILE.GetString("CONFIG", "SOUNDALERT", "") = "TRUE") Then

                    My.Computer.Audio.Play(My.Resources.Alert, AudioPlayMode.Background)

                End If

            Else

                Label1.ForeColor = Color.White

            End If

        Catch ex As Exception



        End Try

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        sound_click()
        SETTINGS.SETTINGS_Load()
        SETTINGS.Show()
        Me.Hide()

    End Sub

    Private Sub Label4_MouseClick(sender As Object, e As MouseEventArgs) Handles Label4.MouseClick

        If (e.Button = MouseButtons.Right) Then

            Clipboard.SetText(Label4.Text)
            MsgBox("Copied " & "(" & Label4.Text & ")", MsgBoxStyle.Information)

        End If

    End Sub

    Private Sub Label5_MouseClick(sender As Object, e As MouseEventArgs) Handles Label5.MouseClick

        If (e.Button = MouseButtons.Right) Then

            Clipboard.SetText(Label5.Text)
            MsgBox("Copied " & "(" & Label5.Text & ")", MsgBoxStyle.Information)

        End If

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        sound_click()
        Try

            Process.Start(API_S & "\" & VERSIONS_S & "\server.properties")

        Catch ex As Exception

            MsgBox(ex.Message & "File not found, Install and launch first the server.", MsgBoxStyle.Exclamation)

        End Try

    End Sub

End Class
