import createMiddleware from 'next-intl/middleware';

export default createMiddleware({
  locales: ['no', 'en'],
  defaultLocale: 'no',
  localeDetection: true, // automatisk deteksjon via nettleser
});

export const config = {
  matcher: ['/((?!_next|favicon.ico).*)'],
};
