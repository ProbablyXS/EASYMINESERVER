Imports System.Net
Imports System.IO

Public Class About

    Dim download As New WebClient

    Private Sub About_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        sound_click()
        FormEasyMineServer.Show()
        Me.Hide()
        e.Cancel = True

    End Sub

    Private Sub About_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Label3.Text = FormEasyMineServer.maj

        download.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
        download.Headers.Clear()

    End Sub

    Private Sub Label6_MouseEnter(sender As Object, e As EventArgs) Handles Label6.MouseEnter

        Label6.ForeColor = Drawing.Color.White
        Label6.Text = "Donate ?"
        Label6.Padding = New Padding(90, 0, 0, 0)

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

        Label6.Text = "THANKS YOU <3"
        Label6.Padding = New Padding(40, 0, 0, 0)
        Label6.ForeColor = Drawing.Color.Green
        Process.Start("https://www.paypal.me/HRTMTEAM")

    End Sub

End Class