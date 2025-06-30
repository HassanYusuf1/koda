"use client";

import Link from "next/link";

const Footer = () => {
  return (
    <footer className="bg-gray-900 text-white px-6 md:px-12 lg:px-24 py-10 mt-20">
      <div className="max-w-7xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-10">
        {/* Om oss / Kontakt */}
        <div>
          <h4 className="text-lg font-semibold mb-3">NextPlay</h4>
          <p className="mt-3 text-sm text-gray-400">
            📍 Oslo, Norge <br />
            ✉️ kontakt@nextplay.no
          </p>
        </div>

        {/* Lenker */}
        <div>
          <h4 className="text-lg font-semibold mb-3">Lenker</h4>
          <ul className="space-y-2 text-sm text-gray-300">
            <li>
              <Link href="/privacy" className="hover:text-green-400 transition">Personvern</Link>
            </li>
            <li>
              <Link href="/terms" className="hover:text-green-400 transition">Vilkår</Link>
            </li>
            <li>
              <Link href="/about" className="hover:text-green-400 transition">Om oss</Link>
            </li>
          </ul>
        </div>

        {/* Språkvelger */}
        <div>
          <h4 className="text-lg font-semibold mb-3">Språk</h4>
          <select
            className="bg-gray-800 text-sm text-white border border-gray-600 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-green-500"
            defaultValue="no"
            onChange={(e) => {
              const lang = e.target.value;
              // Legg til språkbytte-logikk her (lokal lagring, cookies eller i18n lib)
              console.log("Bytter språk til:", lang);
            }}
          >
            <option value="no">🇳🇴 Norsk</option>
            <option value="en">🇬🇧 English</option>
          </select>
        </div>
      </div>

      {/* Bunntekst */}
      <div className="mt-10 text-center text-xs text-gray-500 border-t border-gray-700 pt-6">
        &copy; {new Date().getFullYear()} NextPlay. Alle rettigheter reservert.
      </div>
    </footer>
  );
};

export default Footer;
