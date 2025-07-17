import React from "react";
import { FaCheckCircle } from "react-icons/fa";
import { IoIosWarning } from "react-icons/io";

interface PopupProps {
  message: string;
  onClose: () => void;
  mode: string;
}

const Popup: React.FC<PopupProps> = ({ message, onClose, mode }) => {
  return (
    <div style={overlayStyle}>
      <div style={popupStyle}>
        <div style={{ display: "flex", flexDirection: "row", marginBottom: "15px" }}>
          <div style={iconStyle}>
            {mode == "success" ? <FaCheckCircle size={16}/> : <IoIosWarning size={20} />}
          </div>
          <p style={{margin: "0px"}}>{message}</p>
        </div>
        <button onClick={onClose}>Cerrar</button>
      </div>
    </div>
  );
};

const iconStyle: React.CSSProperties = {
  width: "24px",
  height: "24px",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  padding: 0,
  margin: "0px 5px 0px 0px"
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
  outline: "rgba(255, 255, 255, 0.57) 1px solid",
  textAlign: "center",
  boxShadow: "0 0 10px rgba(0,0,0,0.25)",
};

export default Popup;
