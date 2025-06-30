"use client";
import Link from "next/link";

const CTASection = () => {
  return (
    <section className="bg-gray-50 text-gray-900 py-24 px-6 md:px-16">
      <div className="max-w-4xl mx-auto text-center">
        <h2 className="text-4xl font-extrabold mb-6">
          Klar for å ta laget ditt til neste nivå?
        </h2>
        <p className="text-lg text-gray-700 mb-10">
          NextPlay gir deg verktøyene for å planlegge smartere, følge opp spillerne bedre og bygge et sterkere lag.
        </p>

        <Link
          href="/register"
          className="inline-block bg-green-500 text-white font-bold py-3 px-8 rounded-full shadow hover:bg-green-600 transition"
        >
          Kom i gang nå
        </Link>
      </div>
    </section>
  );
};

export default CTASection;
