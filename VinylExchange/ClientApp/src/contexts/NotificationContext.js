import React, { createContext } from "react";
import uuidv4 from "../functions/guidGenerator";
export const NotificationContext = createContext();

export default class NotificationContextProvider extends React.Component {
  state = {
    messages: [],
    severity: 0
  };

  handleServerNotification = (notificationObj, customMessage) => {
    console.log(notificationObj);
    let severity = 1;

   if (notificationObj.status >= 400) {
      const errorMessages = [];

      if (typeof notificationObj.data ==="object" && notificationObj.data.errors != undefined) {
        const errors = notificationObj.data.errors;

        Object.keys(errors).forEach(function(field) {
          errorMessages.push({
            messageText: `${field} : ${errors[field].join()}`,
            id: uuidv4()
          });
        });
      }else if ((Array.isArray(notificationObj.data))) {
        severity = 2;
        notificationObj.data.forEach(warn=>{
          errorMessages.push({
            messageText: warn.description,
            id: uuidv4()
          });
        });
      } else {
        if (customMessage != undefined) {
          errorMessages.push({
            messageText: customMessage,
            id: uuidv4()
          });
        } else {
          errorMessages.push({
            messageText:
              notificationObj.status + " " + notificationObj.data.title,
            id: uuidv4()
          });
        }
      }

      this.setState({
        messages: errorMessages,
        severity
      });
    } else {
      const successMessages = [];
      if (customMessage != undefined) {
        successMessages.push({
          messageText: customMessage,
          id: uuidv4()
        });
      } else {
        successMessages.push({
          messageText: notificationObj.data.message,
          id: uuidv4()
        });
      }

      this.setState({
        messages: successMessages,
        severity: 3
      });
    }
  };

  handleAppNotification = (message, severity) => {
    this.setState({
      messages: [{ messageText: message, id: uuidv4() }],
      severity
    });
  };

  render() {
    return (
      <NotificationContext.Provider
        value={{
          ...this.state,
          handleServerNotification: this.handleServerNotification,
          handleAppNotification: this.handleAppNotification
        }}
      >
        {this.props.children}
      </NotificationContext.Provider>
    );
  }
}
