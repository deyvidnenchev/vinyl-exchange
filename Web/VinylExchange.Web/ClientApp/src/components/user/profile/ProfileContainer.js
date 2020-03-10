import React, { Component } from "react";
import ProfileComponent from "./ProfileComponent";

class ProfileContainer extends Component {
  constructor(props) {
    super(props);
    this.state = {
      shouldAvatarUpdate: false
    };
  }

  handleShouldAvatarUpdate = () => {
    this.setState(prevState => {
      return{
        shouldAvatarUpdate: prevState.shouldAvatarUpdate ? false : true
      }
    });
  };

  render() {
    return (
      <ProfileComponent
        functions={{ handleShouldAvatarUpdate: this.handleShouldAvatarUpdate }}
        data={{ shouldAvatarUpdate: this.state.shouldAvatarUpdate }}
      />
    );
  }
}

export default ProfileContainer;