import React, { useState } from "react";
// import axios from "axios";
import api from "../api/axios";
import type { UploadFormFields } from "../types/FormData";
import Popup from "./Popup";
import { FaMagnifyingGlass } from "react-icons/fa6";
import KeywordListPopup from "./KeywordListPopup";
import LogProcessPopup from "./LogProcessPopup";
import { FaSpinner } from "react-icons/fa";
import "./UploadForm.css";

const UploadForm: React.FC = () => {
  const [formData, setFormData] = useState<UploadFormFields>({
    keyword: "",
    docName: "",
    file: null,
  });
  const [showPopup, setShowPopup] = useState(false);
  const [popupMessage, setPopupMessage] = useState("");
  const [popupMode, setPopupMode] = useState("");
  const [showKeywordPopup, setShowKeywordPopup] = useState(false);
  const [showLogProcessPopup, setShowLogProcessPopup] = useState(false);
  const [loadingLogs, setLoadingLogs] = useState(false);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] ?? null;
    if (file) {
      if (file.type !== "application/pdf") {
        setShowPopup(true);
        setPopupMessage("Solo se permiten archivos PDF.");
        setPopupMode("warning");
        return;
      }

      const sizeInMB = file.size / (1024 * 1024);
      if (sizeInMB > 10) {
        setShowPopup(true);
        setPopupMessage("El archivo supera los 10MB.");
        setPopupMode("warning");
        return;
      }
    }
    setFormData(prev => ({ ...prev, file }));
  };

  const resetFormData = () => {
    setFormData({
      keyword: "",
      docName: "",
      file: null,
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoadingLogs(true);

    const data = new FormData();
    if (formData.file) {
      data.append("file", formData.file);
    }
    data.append("Keyword", formData.keyword);
    data.append("DocName", formData.docName);

    try {
      const response = await api.post("/form", data, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      if (response.status == 200) {
        resetFormData();
        setShowPopup(true);
        setPopupMessage(response.data.mensaje);
        setPopupMode("success");
      }
    } catch (err) {
      console.error(err);
      alert("Ocurrió un error al enviar los datos.");
    } finally {
      setLoadingLogs(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} style={formStyle}>
      <h2>Subir archivo PDF</h2>

      <div style={inputGroupStyle}>
        <label htmlFor="fileInput" style={fileInputLabelStyle}>
          {formData.file ? formData.file.name : "Selecciona un archivo PDF"}
        </label>
        <input
          id="fileInput"
          type="file"
          accept="application/pdf"
          onChange={handleFileChange}
          style={hiddenFileInputStyle}
        />
      </div>

      <div style={inputGroupStyle}>
        <div style={{ display: "flex", flexDirection: "row" }}>
          <label style={{ flex: "auto" }}>Palabra clave:</label>
          <button
            type="button"
            onClick={() => setShowKeywordPopup(true)}
            style={{
              border: "none",
              background: "none",
              color: "#1d1d1dff",
              cursor: "pointer",
              width: "32px",
              height: "32px",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              padding: 0,
            }}
            title="Ver palabras clave guardadas"
          >
            <FaMagnifyingGlass size={16} />
          </button>
        </div>
        <textarea
          name="keyword"
          required
          onChange={handleChange}
          value={formData.keyword}
          rows={3}
        />
      </div>

      <div style={inputGroupStyle}>
        <label>Nombre de archivo nuevo:</label>
        <input
          name="docName"
          type="text"
          required
          onChange={handleChange}
          value={formData.docName}
        />
      </div>

      <button type="submit" disabled={loadingLogs}>
        {loadingLogs ? (
          <FaSpinner style={{ color: "#fff" }} className="spin" />
        ) : (
          "Enviar"
        )}
      </button>
      <button onClick={() => setShowLogProcessPopup(true)} type="button">
        Mostrar Registros
      </button>
      {showPopup && (
        <Popup mode={popupMode} message={popupMessage} onClose={() => setShowPopup(false)} />
      )}
      {showKeywordPopup && (
        <KeywordListPopup onClose={() => setShowKeywordPopup(false)} />
      )}
      {showLogProcessPopup && (
        <LogProcessPopup onClose={() => setShowLogProcessPopup(false)} />
      )}
    </form>
  );
};

const hiddenFileInputStyle: React.CSSProperties = {
  display: "none",
};

const fileInputLabelStyle: React.CSSProperties = {
  padding: "10px",
  border: "1px dashed #fff9db",
  borderRadius: "6px",
  cursor: "pointer",
  backgroundColor: "#655fb4",
  textAlign: "center",
  color: "#fff",
};

const formStyle: React.CSSProperties = {
  maxWidth: "400px",
  margin: "50px auto",
  padding: "20px",
  border: "1px solid #ccc",
  borderRadius: "8px",
  display: "flex",
  flexDirection: "column",
  gap: "15px",
};

const inputGroupStyle: React.CSSProperties = {
  display: "flex",
  flexDirection: "column",
};

export default UploadForm;
