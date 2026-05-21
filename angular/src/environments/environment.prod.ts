import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'MyEcommerce',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44399/',
    redirectUri: baseUrl,
    clientId: 'MyEcommerce_App',
    responseType: 'code',
    scope: 'offline_access MyEcommerce',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44360',
      rootNamespace: 'MyEcommerce',
    },
  },
} as Environment;
