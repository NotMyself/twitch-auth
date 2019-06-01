function (user, context, callback) {
  const request = require('request');
  const claimNamespace = 'https://iamnotmyself.com/';
  const userSearchApiUrl = auth0.baseUrl + '/users-by-email';

  if (!user.email || context.clientMetadata.includeIdps !== 'true') {
    return callback(null, user, context);
  }

  request({
    url: userSearchApiUrl,
    headers: {
      Authorization: 'Bearer ' + auth0.accessToken
    },
    qs: {
      email: user.email
    }
  }, function(err, response, body) {
    if (err) return callback(err);
    if (response.statusCode !== 200) return callback(new Error(body));
    var profiles = JSON.parse(body);

    if(profiles.length !== 1) {
      callback(null, user, context);
    }

    var idps = profiles[0].identities.reduce((a, identity) => {
      if(identity.provider === 'oauth2') {
        a[identity.connection] = identity.access_token;
      }
      return a;
    }, {});

    context.idToken[`${claimNamespace}connections`] = idps;

    callback(null, user, context);
  });
}
