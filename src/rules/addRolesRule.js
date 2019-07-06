function (user, context, callback) {
  const namespace = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/';
  const tokenUrl = `https://${auth0.domain}/oauth/token`;
  const rolesUrl = `${auth0.baseUrl}/users/${user.user_id}/roles`;
  const clientId = configuration.clientId;
  const clientSecret = configuration.clientSecret;
  
  if (context.clientMetadata.includeRoles !== 'true') {
    return callback(null, user, context);
  }
  
  request({ method: 'POST',
    url: tokenUrl,
    headers: { 
      'content-type': 'application/x-www-form-urlencoded' 
    },
    json: true,
    form: 
     { 
       grant_type: 'client_credentials',
       client_id: clientId,
       client_secret: clientSecret,
       audience: auth0.baseUrl + '/'
     } 
  },(err, res, body) => {
    request({ 
      url: rolesUrl,
      json: true,
      headers: {
        authorization: `Bearer ${body.access_token}`
      }
    }, (err, res, body) => {
      context.idToken[namespace + "roles"] = body.map(c => c.name);
      callback(null, user, context);
    });
  });  
}