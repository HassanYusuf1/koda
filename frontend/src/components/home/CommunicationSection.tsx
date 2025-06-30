"use client";

const CommunicationSection = () => {
  return (
    <section className="bg-white py-24 px-6 md:px-16 text-gray-900 space-y-24">
      {/* Første blokk */}
      <div className="max-w-7xl mx-auto flex flex-col md:flex-row items-center gap-12">
        <div className="w-full md:w-1/2">
          <img
            src="https://unsplash.it/600/400?image=1031"
            alt="Kommunikasjon i laget"
            className="rounded-xl shadow-lg w-full h-auto"
          />
        </div>
        <div className="w-full md:w-1/2">
          <h2 className="text-3xl font-bold mb-4">Alt på ett sted</h2>
          <p className="text-lg text-gray-700 mb-4">
            Kommunikasjon mellom trenere, spillere og støtteapparat skjer sømløst i NextPlay. Del mål, tilbakemeldinger og endringer – uten å miste oversikten i meldingskaos.
          </p>
          <p className="text-md text-gray-600">
            Du trenger ikke lenger lete i chatgrupper – alt lagres og deles der det hører hjemme: sammen med data og analyser.
          </p>
        </div>
      </div>

      {/* Andre blokk – speilvendt */}
      <div className="max-w-7xl mx-auto flex flex-col md:flex-row-reverse items-center gap-12">
        <div className="w-full md:w-1/2">
          <img
            src="https://unsplash.it/600/400?image=1055"
            alt="Trener gir tilbakemelding"
            className="rounded-xl shadow-lg w-full h-auto"
          />
        </div>
        <div className="w-full md:w-1/2">
          <h2 className="text-3xl font-bold mb-4">Bedre spillerutvikling</h2>
          <p className="text-lg text-gray-700 mb-4">
            Spillere får rask og tydelig tilbakemelding etter økter. Det gjør utviklingen målbar – og personlig.
          </p>
          <p className="text-md text-gray-600">
            NextPlay gjør det lett å følge opp hver enkelt, uten ekstra arbeid. Alt skjer i samme system.
          </p>
        </div>
      </div>
    </section>
  );
};

export default CommunicationSection;
