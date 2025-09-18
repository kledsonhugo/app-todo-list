@description('SQL Database ID for role assignment')
param sqlDatabaseId string

@description('Managed Identity Principal ID')
param managedIdentityPrincipalId string

@description('Managed Identity ID for GUID generation')
param managedIdentityId string

// Role assignment for Managed Identity to access SQL Database
resource sqlDbContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: resourceId('Microsoft.Sql/servers/databases', split(sqlDatabaseId, '/')[8], split(sqlDatabaseId, '/')[10])
  name: guid(sqlDatabaseId, managedIdentityId, 'SQL DB Contributor')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec') // SQL DB Contributor
    principalId: managedIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Outputs
output roleAssignmentId string = sqlDbContributorRoleAssignment.id