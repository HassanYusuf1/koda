"use client";

import Image from "next/image";
import Link from "next/link";
import footballImage from "../../../public/image/football-hero.jpg";

const HeroSection = () => {
  return (
    <section className="relative bg-gradient-to-br from-black via-gray-900 to-green-900 text-white py-24 px-6 md:px-16 flex flex-col md:flex-row items-center justify-between overflow-hidden">
      
      {/* Dekorativ grønn bakgrunnssirkel */}
      <div className="absolute top-[-100px] left-[-100px] w-[300px] h-[300px] bg-green-500 opacity-30 rounded-full blur-3xl z-0"></div>

      {/* Tekstinnhold */}
      <div className="z-10 max-w-xl space-y-6">
        <h1 className="text-5xl font-extrabold leading-tight tracking-tight">
          Ta laget ditt <span className="text-green-400">til neste nivå</span>
        </h1>
        <p className="text-lg text-gray-300">
          NextPlay gir trenere full oversikt og hjelper klubber med å ta smartere beslutninger basert på fakta.
        </p>
        <ul className="space-y-2 text-gray-100 text-base font-light">
          <li>⚽ Se fremgang for hver spiller</li>
          <li>📊 Del detaljerte rapporter med støtteapparatet</li>
          <li>🧠 Planlegg smartere treningsøkter</li>
        </ul>
        <Link
          href="/register"
          className="mt-6 inline-flex items-center bg-green-500 hover:bg-green-400 text-black font-bold py-3 px-6 rounded-full shadow-lg transition"
        >
          Kom i gang nå
          <span className="ml-2 text-xl">→</span>
        </Link>
      </div>

      {/* Bilde */}
      <div className="w-full md:w-1/2 mt-12 md:mt-0 z-10">
        <Image
          src={footballImage}
          alt="Fotballspiller"
          className="w-full h-auto rounded-xl ring-4 ring-green-500/20 shadow-2xl"
          priority
        />
      </div>
    </section>
  );
};

export default HeroSection;
