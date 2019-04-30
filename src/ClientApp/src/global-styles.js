import { createGlobalStyle } from 'styled-components';
import 'reset-css/reset.css';

const GlobalStyle = createGlobalStyle`
  html,
  body {
    height: 100%;
    width: 100%;
    line-height: 1.5;
    background: #fff;
    font-family: source sans pro,sans-serif;
    font-size: 1rem;
    color: #373a3c;
  }

  #app {
    background-color: #fafafa;
    min-height: 100%;
    min-width: 100%;
  }

  .container {
    margin-left: auto;
    margin-right: auto;
    padding-left: 15px;
    padding-right: 15px;
  }
`;

export default GlobalStyle;
