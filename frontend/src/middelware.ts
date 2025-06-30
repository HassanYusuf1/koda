import createMiddleware from 'next-intl/middleware';
import {locales, defaultLocale} from './lib/i18n';

export default createMiddleware({
  locales,
  defaultLocale,
});

export const config = {
  matcher: ['/', '/(nb|en)/:path*']
};
