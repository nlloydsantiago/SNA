Option Explicit On
Option Strict On
Imports Aptify.Framework.BusinessLogic.GenericEntity
Imports Aptify.Applications.OrderEntry

Public Class SNASetSubscriptionStartDate
    Inherits Aptify.Applications.OrderEntry.Subscriptions.SubscriptionDate
    Public Overrides Function GetSubsStartDate(SubscriberID As Long, SubscriberCompanyID As Long, ProductID As Long, OrderDate As Date, ByRef SubscriptionStartDate As Date, OrderGE As OrdersEntity, OrderLineGE As OrderLinesEntity) As Boolean
        Try
            MyBase.GetSubsStartDate(SubscriberID, SubscriberCompanyID, ProductID, OrderDate, SubscriptionStartDate, OrderGE, OrderLineGE)
            Dim productGE As AptifyGenericEntityBase = Application.GetEntityObject("Products", ProductID)
            If Convert.ToString(productGE.GetValue("IsDefaultSubscriptionStartDate_sna")) = "1" Then
                'If Convert.ToString(OrderLineGE.ProductData.GetValue("IsDefaultSubscriptionStartDate_sna")) = "1" Then
                SubscriptionStartDate = New DateTime(Today.Year, Today.Month, 1)
            End If
        Catch ex As Exception
            Aptify.Framework.ExceptionManagement.ExceptionManager.Publish(ex)
            Return False
        End Try
        Return True
    End Function
End Class
