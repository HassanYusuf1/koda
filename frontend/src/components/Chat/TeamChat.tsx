"use client";

import { useRef, useState } from "react";
import { Paperclip, Send, Smile } from "lucide-react";

interface Member {
  id: number;
  name: string;
}

interface Attachment {
  id: number;
  file: File;
  url: string;
}

interface Message {
  id: number;
  sender: string;
  text: string;
  attachments?: Attachment[];
}

const members: Record<string, Member[]> = {
  "Trenere": [
    { id: 1, name: "Ola Nordmann" },
  ],
  "Spillere": [
    { id: 2, name: "Kari Nordmann" },
    { id: 3, name: "Per Hansen" },
  ],
  "Støtteapparat": [
    { id: 4, name: "Lise Olsen" },
  ],
};

export default function TeamChat() {
  const [messages, setMessages] = useState<Message[]>([
    { id: 1, sender: "Ola", text: "Hei laget!" },
    { id: 2, sender: "Kari", text: "Klar for trening?" },
  ]);
  const [input, setInput] = useState("");
  const [attachments, setAttachments] = useState<Attachment[]>([]);
  const [showEmoji, setShowEmoji] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleSend = () => {
    if (!input.trim() && attachments.length === 0) return;
    setMessages((prev) => [
      ...prev,
      { id: Date.now(), sender: "Meg", text: input, attachments },
    ]);
    setInput("");
    setAttachments([]);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files) return;
    const newAtt = Array.from(files).map((file) => ({
      id: Date.now() + Math.random(),
      file,
      url: URL.createObjectURL(file),
    }));
    setAttachments((prev) => [...prev, ...newAtt]);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const addEmoji = (emoji: string) => {
    setInput((prev) => prev + emoji);
    setShowEmoji(false);
  };

  return (
    <div className="flex h-[calc(100vh-8rem)] bg-gray-900 rounded-xl overflow-hidden">
      <aside className="w-64 bg-gray-800 border-r border-gray-700 p-4 overflow-y-auto">
        {Object.entries(members).map(([role, list]) => (
          <div key={role} className="mb-6">
            <h3 className="text-sm font-semibold text-gray-400 uppercase mb-2">
              {role}
            </h3>
            <ul className="space-y-1">
              {list.map((m) => (
                <li key={m.id}>
                  <button className="w-full text-left px-2 py-1 rounded hover:bg-gray-700 text-white">
                    {m.name}
                  </button>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </aside>
      <section className="flex-1 flex flex-col">
        <div className="flex-1 overflow-y-auto p-6 space-y-4 bg-gray-950">
          {messages.map((msg) => (
            <div key={msg.id} className="flex flex-col">
              <span className="text-sm text-gray-400">{msg.sender}</span>
              {msg.text && (
                <p className="bg-gray-800 p-3 rounded-lg w-fit max-w-md text-white">
                  {msg.text}
                </p>
              )}
              {msg.attachments && (
                <div className="mt-2 flex gap-2 flex-wrap">
                  {msg.attachments.map((att) => (
                    <div key={att.id} className="text-sm text-gray-300">
                      {att.file.type.startsWith("image") ? (
                        <img src={att.url} alt={att.file.name} className="h-20 rounded" />
                      ) : (
                        <a href={att.url} download className="underline text-blue-400">
                          {att.file.name}
                        </a>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>
          ))}
        </div>
        <div className="relative border-t border-gray-700 p-4 bg-gray-800 flex flex-col gap-2">
          {attachments.length > 0 && (
            <div className="flex gap-2 flex-wrap">
              {attachments.map((att) => (
                <div key={att.id} className="text-sm text-gray-300 relative">
                  {att.file.type.startsWith("image") ? (
                    <img src={att.url} alt={att.file.name} className="h-12 rounded" />
                  ) : (
                    <span className="px-2 py-1 bg-gray-700 rounded">{att.file.name}</span>
                  )}
                </div>
              ))}
            </div>
          )}
          <div className="flex items-center gap-2">
            <button
              className="p-2 rounded hover:bg-gray-700"
              aria-label="Emoji"
              onClick={() => setShowEmoji(!showEmoji)}
            >
              <Smile size={20} />
            </button>
            {showEmoji && (
              <div className="absolute bottom-14 bg-gray-800 border border-gray-700 rounded p-2 grid grid-cols-7 gap-1">
                {["😀","😅","🥳","😢","❤️","👍","🔥","😂","🤔","👏"].map((e) => (
                  <button key={e} className="text-xl" onClick={() => addEmoji(e)}>
                    {e}
                  </button>
                ))}
              </div>
            )}
            <button
              className="p-2 rounded hover:bg-gray-700"
              aria-label="Vedlegg"
              onClick={() => fileInputRef.current?.click()}
            >
              <Paperclip size={20} />
            </button>
            <input
              type="file"
              multiple
              ref={fileInputRef}
              onChange={handleFileChange}
              className="hidden"
            />
          <input
            type="text"
            className="flex-1 bg-gray-700 rounded p-2 text-white placeholder-gray-400 focus:outline-none"
            placeholder="Skriv en melding..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") handleSend();
            }}
          />
          <button
            onClick={handleSend}
            className="p-2 bg-green-600 hover:bg-green-500 rounded text-white"
            aria-label="Send"
          >
            <Send size={20} />
          </button>
          </div>
        </div>
      </section>
    </div>
  );
}