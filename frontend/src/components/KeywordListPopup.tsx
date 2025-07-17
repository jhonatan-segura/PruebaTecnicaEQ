import React, { useEffect, useState } from "react";
import api from "../api/axios";
import { IoMdClose } from "react-icons/io";
import Popup from "./Popup";

interface KeywordListPopupProps {
  onClose: () => void;
}

interface DocKeyword {
  id: number;
  docName: string;
  keyword: string;
}

const KeywordListPopup: React.FC<KeywordListPopupProps> = ({ onClose }) => {
  const [keywords, setKeywords] = useState<DocKeyword[]>([]);
  const [loading, setLoading] = useState(true);
  const [showPopup, setShowPopup] = useState(false);
  const [popupMessage, setPopupMessage] = useState("");
  const [popupMode, setPopupMode] = useState("");

  useEffect(() => {
    api
      .get("/DocKeyword")
      .then(response => {
        setKeywords(response.data);
        setLoading(false);
      })
      .catch(error => {
        setShowPopup(true);
        setPopupMessage(`Error al obtener las palabras clave: ${error}`);
        setPopupMessage("warning");
        setLoading(false);
      });
  }, []);

  const handleDeletion = async (id: number) => {
    try {
      const response = await api.delete(`/DocKeyword/${id}`);

      if (response.status === 200) {
        setKeywords(prev => prev.filter(item => item.id !== id));
      }
    } catch (error) {
      setShowPopup(true);
      setPopupMessage(`Error al eliminar la palabra clave: ${error}`);
      setPopupMode("warning");
    }
  };

  const updateKeywordDebounced = (() => {
    let timer: ReturnType<typeof setTimeout>;

    return (id: number, newKeyword: string, docName: string) => {
      clearTimeout(timer);

      timer = setTimeout(async () => {
        try {
          await api.put(`/DocKeyword/${id}`, {
            id,
            keyword: newKeyword,
            docName: docName,
          });
        } catch (error) {
          setShowPopup(true);
          setPopupMessage(`Error al actualizar: ${error}`);
          setPopupMode("warning");
        }
      }, 600);
    };
  })();

  const handleKeywordChange = (
    id: number,
    newKeyword: string,
    docName: string
  ) => {
    setKeywords(prev =>
      prev.map(item =>
        item.id === id ? { ...item, keyword: newKeyword } : item
      )
    );

    updateKeywordDebounced(id, newKeyword, docName);
  };

  return (
    <div style={overlayStyle}>
      <div style={popupStyle}>
        <h2>Palabras clave guardadas</h2>
        {loading ? (
          <p>Cargando...</p>
        ) : (
          <div>
            {keywords.map((item, index) => (
              <div
                key={index}
                style={{
                  display: "flex",
                  flexDirection: "row",
                  marginBottom: "5px",
                  paddingRight: "10px",
                }}
              >
                <input
                  type="text"
                  style={{
                    flex: "auto",
                    alignSelf: "center",
                    padding: "3px 5px",
                    border: "#6f6b7b 1px solid",
                    fontSize: "15px",
                  }}
                  value={item.keyword}
                  onChange={e =>
                    handleKeywordChange(item.id, e.target.value, item.docName)
                  }
                />
                <button
                  type="button"
                  onClick={() => handleDeletion(item.id)}
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
                  title="Eliminar palabra clave."
                >
                  <IoMdClose size={16} />
                </button>
              </div>
            ))}
          </div>
        )}
        <button style={{ marginTop: "15px" }} onClick={onClose}>
          Cerrar
        </button>
      </div>
      {showPopup && (
        <Popup mode={popupMode} message={popupMessage} onClose={() => setShowPopup(false)} />
      )}
    </div>
  );
};

const overlayStyle: React.CSSProperties = {
  position: "fixed",
  top: 0,
  left: 0,
  width: "100vw",
  height: "100vh",
  backgroundColor: "rgba(0,0,0,0.5)",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  zIndex: 1000,
};

const popupStyle: React.CSSProperties = {
  background: "#fff9db",
  padding: "20px",
  borderRadius: "8px",
  width: "400px",
  maxHeight: "80vh",
  overflowY: "auto",
  boxShadow: "0 0 10px rgba(0,0,0,0.2)",
  outline: "rgba(255, 255, 255, 0.57) 1px solid",
};

export default KeywordListPopup;
