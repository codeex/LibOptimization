﻿Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' optimize template
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsOptTemplate : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' epsilon(Default:1e-8) for Criterion
        ''' </summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>
        ''' Max iteration count(Default:10,000)
        ''' </summary>
        Public Property Iteration As Integer = 10000

        ''' <summary>
        ''' Range of initial value(Default:+-5)
        ''' </summary>
        Public Property InitialValueRange As Double = 5 'parameter range

        '----------------------------------------------------------------
        'Peculiar parameter
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:100)
        ''' </summary>
        Public Property PopulationSize As Integer = 100

        'population
        Private m_populations As New List(Of clsPoint)

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_populations.Clear()

                'Set initialize value
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim temp As New List(Of Double)
                    For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        temp.Add(Math.Abs(2.0 * InitialValueRange) * m_rand.NextDouble() - InitialValueRange)
                    Next
                    Me.m_populations.Add(New clsPoint(MyBase.m_func, temp))
                Next

                'Sort Evaluate
                Me.m_populations.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_populations.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_populations.Count OrElse Me.HigherNPercentIndex >= Me.m_populations.Count Then
                    Me.HigherNPercentIndex = Me.m_populations.Count - 1
                End If

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                Me.m_populations.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_populations(0).Eval, Me.m_populations(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.Iteration <= Me.m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'add logic
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return Me.m_populations(0)
            End Get
        End Property

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property ResultForDebug As List(Of clsPoint)
            Get
                Return Me.m_populations
            End Get
        End Property
#End Region

#Region "Private"

#End Region
    End Class
End Namespace
