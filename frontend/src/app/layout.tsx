import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Address Management - Нова Пошта",
  description: "Управління адресами відділень Нової Пошти",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="uk">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}