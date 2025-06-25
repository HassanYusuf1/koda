import "./globals.css";

export const metadata = {
  title: "NextPlay",
  description: "Spillerdashboard",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="no">
      <body className="bg-gray-900 text-white">{children}</body>
    </html>
  );
}
