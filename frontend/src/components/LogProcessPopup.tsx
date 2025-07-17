import React, { useEffect, useState } from "react";
import api from "../api/axios";
import { IoMdRefresh } from "react-icons/io";
import Popup from "./Popup";

interface KeywordListPopupProps {
  onClose: () => void;
}

interface LogProcess {
  id: number;
  originalFileName: string;
  status: string;
  newFileName: string;
  dateProcessed: string;
}

const LogProcessPopup: React.FC<KeywordListPopupProps> = ({ onClose }) => {
  const [logProcesses, setLogProcesses] = useState<LogProcess[]>([]);
  const [loading, setLoading] = useState(true);
  const [showPopup, setShowPopup] = useState(false);
  const [popupMessage, setPopupMessage] = useState("");
  const [popupMode, setPopupMode] = useState("");

  useEffect(() => {
    getAllLogProcesses();
  }, []);

  const getAllLogProcesses = async () => {
    api
      .get("/LogProcess")
      .then(response => {
        console.log(response.data[0].dateProcessed);
        setLogProcesses(response.data);
        setLoading(false);
      })
      .catch(error => {
        setShowPopup(true);
        setPopupMessage(`Error al obtener los registros: ${error}`);
        setPopupMode("warning");
        setLoading(false);
      });
  }

  const formatDate = (d: string) => {
    let date = new Date(d);
    return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
  };

  return (
    <div style={overlayStyle}>
      <div style={popupStyle}>
        <div style={{ display: "flex", flexDirection: "row", marginBottom: "15px" }}>
          <h2 style={{ flex: "auto", margin: "0px" }}>Registros de procesamiento de documentos</h2>
          <button
            type="button"
            onClick={() => getAllLogProcesses()}
            style={{
              border: "none",
              background: "none",
              color: "#1d1d1dff",
              cursor: "pointer",
              width: "36px",
              height: "36px",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              padding: 0,
            }}
            title="Ver palabras clave guardadas"
          >
            <IoMdRefresh size={18} />
          </button>
        </div>
        {loading ? (
          <p>Cargando...</p>
        ) : (
          <div>
            <table style={tableStyle}>
              <thead style={{ background: "#655fb4" }}>
                <tr>
                  <th style={{ padding: "10px" }}>Nombre original</th>
                  <th style={{ width: "150px" }}>Estado</th>
                  <th style={{ width: "200px" }}>Nombre nuevo</th>
                  <th style={{ width: "200px" }}>Fecha de procesamiento</th>
                </tr>
              </thead>
              <tbody>
                {logProcesses.map((item, index) => (
                  <tr
                    style={
                      index % 2 == 0
                        ? {
                            background: "#f1f0cdff",
                            color: "rgba(124, 124, 124, 1)",
                          }
                        : {
                            background: "#e4e3caff",
                            color: "rgba(124, 124, 124, 1)",
                          }
                    }
                  >
                    <td style={{ padding: "10px" }}>{item.originalFileName}</td>
                    <td>{item.status}</td>
                    <td>{item.newFileName}</td>
                    <td>{formatDate(item.dateProcessed)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
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

const tableStyle: React.CSSProperties = {
  borderCollapse: "separate",
  borderSpacing: 0,
  borderRadius: "10px",
  overflow: "hidden",
  color: "white"
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
  width: "900px",
  maxHeight: "80vh",
  overflowY: "auto",
  boxShadow: "0 0 10px rgba(0,0,0,0.2)",
  outline: "rgba(255, 255, 255, 0.57) 1px solid",
};

export default LogProcessPopup;
