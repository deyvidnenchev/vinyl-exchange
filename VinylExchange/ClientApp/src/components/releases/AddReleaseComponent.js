import React,{Component} from "react";
import Label from "./../common/inputComponents/Label";
import TextInput from "./../common/inputComponents/TextInput";
import SingleSelect from "./../common/inputComponents/SingleSelect";
import Select, { components } from "react-select";
import { Url, Controllers } from "./../../constants/UrlConstants";
import $script from "scriptjs";
import { Helmet } from "react-helmet";

 class AddReleaseComponent extends Component {
  constructor() {
    super();
  }

  componentDidMount() {
    $script.get("/js/dropzone.js", function() {
      $script.get("/js/imagesDropzoneScript.js", function() {
        $script.get("/js/tracksDropzoneScript.js", function() {});
      });
    });
  }

  componentWillUnmount() {
    var head = document.getElementsByTagName("head")[0];
    head.removeChild(document.getElementById("imagesDropzoneScript"));
    head.removeChild(document.getElementById("tracksDropzoneScript"));
  }

  render() {
    return (
      <div className="release-form">
        <Helmet>
          <link rel="stylesheet" href="/css/dropzone.css" />
        </Helmet>
        <form onSubmit={event => this.props.functions.handleOnSubmit(event)}>
          <div className="row">
            <div className="col">
              <div className="form-group">
                <Label for="artistInput" value="Artist" />
                <TextInput
                  id="artistInput"
                  placeholder="Artist"
                  value={this.props.state.artistInput}
                  onChange={this.props.functions.handleOnChange}
                />
              </div>
            </div>

            <div className="col">
              <div className="form-group">
                <Label for="titleInput" value="Title" />
                <TextInput
                  id="titleInput"
                  placeholder="Title"
                  value={this.props.state.titleInput}
                  onChange={this.props.functions.handleOnChange}
                />
              </div>
            </div>
          </div>
          <div className="row">
            <div className="col">
              <div className="form-group">
                <Label for="genreSelectInput" value="Genre" />
                <SingleSelect
                  id="genreSelectInput"
                  value={this.props.state.genreSelectInput}
                  onChange={this.props.functions.handleOnChange}
                  options={this.props.state.genres}
                  defaultOptionLabel="--Please Select Genre--"
                />
              </div>
            </div>

            <div className="col">
              <div className="form-group">
                <Label for="styleMultiSelectInput" value="Styles" />
                <Select
                  id="styleMultiSelectInput"
                  closeMenuOnSelect={false}
                  isMulti
                  value={this.props.state.styleMultiSelectInput}
                  onChange={this.props.functions.handleOnChangeMultiSelect}
                  options={this.props.state.styles}
                />
              </div>
            </div>
          </div>

          <div className="row">
            <div className="col">
              <div className="form-group">
                <Label for="formatInput" value="Format" />
                <TextInput
                  id="formatInput"
                  placeholder="Format"
                  value={this.props.state.formatInput}
                  onChange={this.props.functions.handleOnChange}
                />
              </div>
            </div>
            <div className="col">
              <div className="form-group">
                <Label for="yearInput" value="Year" />
                <TextInput
                  id="yearInput"
                  placeholder="Year"
                  value={this.props.state.yearInput}
                  onChange={this.props.functions.handleOnChange}
                />
              </div>
            </div>
            <div className="col">
              <div className="form-group">
                <Label for="labelInput" value="Label" />
                <TextInput
                  id="labelInput"
                  placeholder="Label"
                  value={this.props.state.labelInput}
                  onChange={this.props.functions.handleOnChange}
                />
              </div>
            </div>
          </div>
          <div className="form-group">
            <Label for="release-images-dropzone" value="Release Images" />
            <div
              id="imagesDropzone"
              formsessionid={this.props.state.formSessionId}
              dropzoneuploadpath= {Url.api + Controllers.files.name}
              dropzonedeletepath={Url.api + Controllers.files.name + Url.slash}
              acceptedfiles=".png, .jpg"
            ></div>
          </div>

          <div className="form-group">
            <Label for="release-tracks-dropzone" value="Release Tracks" />
            <div
              id="tracksDropzone"
              formsessionid={this.props.state.formSessionId}
              dropzoneuploadpath= {Url.api + Controllers.files.name }
              dropzonedeletepath={Url.api + Controllers.files.name + Url.slash }
              acceptedfiles=".mp3"
            ></div>
          </div>

          <button type="submit" className="btn btn-primary">
            Submit
          </button>
        </form>
      </div>
    );
  }
}

export default AddReleaseComponent;